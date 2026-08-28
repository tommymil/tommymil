using System.Security.Cryptography;
using LessonRunner.Application.Notifications;
using LessonRunner.Application.Scheduling;
using LessonRunner.Domain.Notifications;
using LessonRunner.Domain.Users;

namespace LessonRunner.Application.Auth;

/// <summary>
/// Reset hasła i zaproszenia oparte na tokenie jednorazowym wysyłanym na e-mail.
///
/// Decyzje warte zapamiętania:
/// - **Żądanie resetu nigdy nie zdradza, czy konto istnieje.** Odpowiedź jest identyczna dla
///   adresu znanego i nieznanego. Inaczej formularz „nie pamiętam hasła” byłby wygodnym
///   sprawdzaczem, którzy rodzice korzystają ze szkoły.
/// - **W bazie leży wyłącznie skrót tokenu.** Postać jawna istnieje tylko w wysłanym e-mailu.
/// - **Wydanie nowego tokenu unieważnia poprzednie.** W skrzynce może leżeć kilka wiadomości;
///   działa najnowsza.
/// - **Te wiadomości omijają przełączniki powiadomień i warunek zgody RODO.** To poczta
///   dotycząca dostępu do konta, a nie informacja o zajęciach — wyłączenie przypomnień nie może
///   odbierać rodzicowi możliwości zalogowania się. Zapisujemy je natomiast w dzienniku wysyłek.
/// - **Treść jest wbudowana, nie edytowalna w panelu.** Szablon zepsuty literówką w panelu
///   oznaczałby, że nikt nie odzyska hasła; a jedyne, co tu naprawdę trzeba podmienić - nadawcę -
///   bierzemy z ustawień powiadomień.
/// </summary>
public sealed class AccountTokenService(
    IUserRepository userRepository,
    IAccountTokenRepository tokenRepository,
    IPasswordHasher passwordHasher,
    IEmailSender emailSender,
    INotificationRepository notificationRepository,
    AppOptions appOptions) : IAccountTokenService
{
    /// <summary>Ile tokenów wolno wydać jednemu kontu w ciągu godziny. Zabezpieczenie przed
    /// zasypaniem skrzynki rodzica z wielu adresów IP.</summary>
    private const int MaxTokensPerHour = 5;

    public async Task RequestPasswordResetAsync(RequestPasswordResetDto dto, CancellationToken cancellationToken)
    {
        var email = (dto.Email ?? string.Empty).Trim();

        if (email.Length == 0 || !email.Contains('@'))
        {
            return;
        }

        var user = await userRepository.GetByEmailAsync(User.NormalizeEmail(email), cancellationToken);

        // Konto nieznane albo wyłączone: kończymy po cichu. Wyłączone konto celowo nie dostaje
        // linku - reset hasła nie może być obejściem dezaktywacji.
        if (user is null || !user.IsActive)
        {
            return;
        }

        if (await ExceededQuotaAsync(user.Id, cancellationToken))
        {
            return;
        }

        await IssueAndSendAsync(user, AccountTokenPurpose.PasswordReset, issuedByUserId: null, cancellationToken);
    }

    public async Task<AccountTokenInfoDto?> DescribeAsync(string token, CancellationToken cancellationToken)
    {
        var (stored, user) = await ResolveAsync(token, cancellationToken);

        return stored is null || user is null
            ? null
            : new AccountTokenInfoDto(
                stored.Purpose.ToString().ToLowerInvariant(),
                user.Email,
                user.DisplayName,
                stored.ExpiresAt);
    }

    public async Task<Guid?> ConfirmPasswordResetAsync(ConfirmPasswordResetDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            throw new ArgumentException("Hasło musi mieć co najmniej 8 znaków.");
        }

        var (stored, user) = await ResolveAsync(dto.Token, cancellationToken);

        if (stored is null || user is null)
        {
            return null;
        }

        // Zużycie tokenu idzie PRZED zmianą hasła i jest warunkowe: gdyby dwa żądania z tym samym
        // linkiem trafiły równocześnie, drugie dostanie `false` i nie ustawi hasła.
        if (!await tokenRepository.MarkUsedAsync(stored.Id, DateTimeOffset.UtcNow, cancellationToken))
        {
            return null;
        }

        // `SetPasswordHashAsync` odświeża znacznik sesji, więc reset wylogowuje wszystkie
        // urządzenia - także to, na którym siedzi ktoś, kto właśnie przejął konto.
        if (!await userRepository.SetPasswordHashAsync(user.Id, passwordHasher.Hash(dto.Password), cancellationToken))
        {
            return null;
        }

        // Pozostałe linki w skrzynce przestają działać po udanej zmianie hasła.
        await tokenRepository.InvalidateActiveAsync(user.Id, DateTimeOffset.UtcNow, cancellationToken);

        return user.Id;
    }

    public async Task<InvitationResultDto?> SendInvitationAsync(Guid userId, Guid? actingUserId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Konto jest wyłączone — najpierw je aktywuj.");
        }

        if (await ExceededQuotaAsync(user.Id, cancellationToken))
        {
            throw new InvalidOperationException("Zbyt wiele zaproszeń do tego konta w ciągu ostatniej godziny.");
        }

        return await IssueAndSendAsync(user, AccountTokenPurpose.Invitation, actingUserId, cancellationToken);
    }

    private async Task<InvitationResultDto> IssueAndSendAsync(
        User user,
        AccountTokenPurpose purpose,
        Guid? issuedByUserId,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        // Nowy token zastępuje wszystkie poprzednie: w skrzynce może leżeć kilka wiadomości,
        // ale działa najnowsza.
        await tokenRepository.InvalidateActiveAsync(user.Id, now, cancellationToken);

        var raw = CreateRawToken();
        var expiresAt = now.Add(AccountToken.LifetimeFor(purpose));

        await tokenRepository.AddAsync(
            new AccountToken
            {
                UserId = user.Id,
                TokenHash = HashToken(raw),
                Purpose = purpose,
                CreatedAt = now,
                ExpiresAt = expiresAt,
                IssuedByUserId = issuedByUserId
            },
            cancellationToken);

        var settings = await notificationRepository.GetSettingsAsync(cancellationToken);
        var link = BuildLink(raw);
        var (subject, body) = purpose == AccountTokenPurpose.Invitation
            ? InvitationMessage(user, link, expiresAt)
            : PasswordResetMessage(user, link, expiresAt);

        var error = await SendLoggedAsync(
            purpose == AccountTokenPurpose.Invitation ? "invitation" : "password-reset",
            user,
            subject,
            body,
            settings,
            cancellationToken);

        return new InvitationResultDto(error is null, expiresAt, error);
    }

    private async Task<bool> ExceededQuotaAsync(Guid userId, CancellationToken cancellationToken)
    {
        var issued = await tokenRepository.CountIssuedSinceAsync(
            userId,
            DateTimeOffset.UtcNow.AddHours(-1),
            cancellationToken);

        return issued >= MaxTokensPerHour;
    }

    private async Task<(AccountToken? Token, User? User)> ResolveAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (null, null);
        }

        var stored = await tokenRepository.GetByHashAsync(HashToken(token.Trim()), cancellationToken);

        if (stored is null || !stored.IsUsable(DateTimeOffset.UtcNow))
        {
            return (null, null);
        }

        var user = await userRepository.GetByIdAsync(stored.UserId, cancellationToken);

        return user is null || !user.IsActive ? (null, null) : (stored, user);
    }

    /// <summary>Wysyła i zapisuje w dzienniku wysyłek. Zwraca komunikat błędu albo `null`.
    /// Klucz deduplikacji zawiera identyfikator tokenu, więc te wiadomości nigdy nie wypadają
    /// przez deduplikację — każde żądanie resetu ma dotrzeć.</summary>
    private async Task<string?> SendLoggedAsync(
        string type,
        User user,
        string subject,
        string body,
        NotificationSettings settings,
        CancellationToken cancellationToken)
    {
        var log = new NotificationLog
        {
            Type = type,
            Channel = "email",
            Recipient = user.Email,
            Subject = subject,
            Status = "pending",
            DedupeKey = $"{type}:{Guid.NewGuid():N}"
        };

        string? error = null;

        try
        {
            await emailSender.SendAsync(
                new EmailMessage(settings.FromEmail, settings.FromName, user.Email, subject, body),
                cancellationToken);
            log.Status = "sent";
            log.SentAt = DateTimeOffset.UtcNow;
        }
        catch (Exception ex)
        {
            log.Status = "failed";
            log.Error = ex.Message;
            error = ex.Message;
        }

        await notificationRepository.AddLogAsync(log, cancellationToken);
        return error;
    }

    private (string Subject, string Body) PasswordResetMessage(User user, string link, DateTimeOffset expiresAt)
    {
        var subject = "Ustawienie nowego hasła";
        var body =
            $"Dzień dobry,\n\notrzymaliśmy prośbę o ustawienie nowego hasła do konta {user.Email}.\n"
            + $"Aby je ustawić, otwórz poniższy link:\n\n{link}\n\n"
            + $"Link jest ważny do {FormatDeadline(expiresAt)} i zadziała tylko raz.\n"
            + "Jeśli to nie Ty prosiłeś o zmianę, zignoruj tę wiadomość — hasło pozostanie bez zmian.\n\n"
            + Signature();

        return (subject, body);
    }

    private (string Subject, string Body) InvitationMessage(User user, string link, DateTimeOffset expiresAt)
    {
        var subject = "Zaproszenie do panelu rodzica";
        var body =
            $"Dzień dobry,\n\nzałożyliśmy dla Państwa konto w systemie zajęć ({user.Email}).\n"
            + "Znajdą tam Państwo harmonogram zajęć, link do spotkania, frekwencję i rozliczenia.\n"
            + $"Aby ustawić hasło i zalogować się po raz pierwszy, otwórz poniższy link:\n\n{link}\n\n"
            + $"Link jest ważny do {FormatDeadline(expiresAt)} i zadziała tylko raz.\n"
            + "Jeśli straci ważność, poproś nas o nowe zaproszenie.\n\n"
            + Signature();

        return (subject, body);
    }

    private static string Signature() => "Szkoła Programowania";

    /// <summary>
    /// Godzina wygaśnięcia linku według zegara szkoły. `ToLocalTime()` brałoby strefę serwera,
    /// czyli UTC w kontenerze — rodzic dostawał termin o dwie godziny wcześniejszy niż faktyczny.
    /// </summary>
    private static string FormatDeadline(DateTimeOffset expiresAt) =>
        SchoolTime.FormatDateTime(expiresAt);

    private string BuildLink(string rawToken) =>
        $"{appOptions.PublicOrigin.TrimEnd('/')}/set-password?token={Uri.EscapeDataString(rawToken)}";

    /// <summary>32 bajty z generatora kryptograficznego, zapisane base64url — bez znaków, które
    /// wymagałyby kodowania w adresie i bez ryzyka, że klient poczty rozetnie link.</summary>
    private static string CreateRawToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

    private static string HashToken(string rawToken) =>
        Convert.ToHexStringLower(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken)));
}
