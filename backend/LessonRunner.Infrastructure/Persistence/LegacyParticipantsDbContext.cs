using LessonRunner.Infrastructure.Groups;
using Microsoft.EntityFrameworkCore;

namespace LessonRunner.Infrastructure.Persistence;

/// <summary>
/// Dodatkowy, minimalny DbContext widzący tylko starą tabelę "GroupParticipants" (uczestnik
/// zaszyty 1:1 w grupie). Używany wyłącznie do jednorazowego backfillu - celowo nie jest częścią
/// <see cref="AppDbContext"/>, żeby migracje EF nigdy nie próbowały zarządzać tą (już istniejącą)
/// tabelą. Tabela jest do usunięcia w kolejnej, osobnej migracji porządkującej.
/// </summary>
internal sealed class LegacyParticipantsDbContext(DbContextOptions<LegacyParticipantsDbContext> options) : DbContext(options)
{
    internal DbSet<LegacyGroupParticipantDocument> GroupParticipants => Set<LegacyGroupParticipantDocument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LegacyGroupParticipantDocument>(builder =>
        {
            builder.ToTable("GroupParticipants");
            builder.HasKey(participant => participant.Id);
        });
    }
}
