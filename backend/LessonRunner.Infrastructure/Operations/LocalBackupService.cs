using System.IO.Compression;
using System.Text.Json;
using LessonRunner.Application.Files;
using LessonRunner.Application.Operations;
using LessonRunner.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LessonRunner.Infrastructure.Operations;

internal sealed class LocalBackupService(
    AppDbContext dbContext,
    IConfiguration configuration,
    IOptions<FileStorageOptions> fileStorageOptions,
    IOptions<BackupOptions> backupOptions) : IBackupService
{
    public async Task<BackupResultDto> CreateAsync(CancellationToken cancellationToken)
    {
        var root = Path.GetFullPath(backupOptions.Value.RootPath);
        Directory.CreateDirectory(root);

        var createdAt = DateTimeOffset.UtcNow;
        var fileName = $"lesson-runner-backup-{createdAt:yyyyMMdd-HHmmss}.zip";
        var path = Path.Combine(root, fileName);

        await using (var fileStream = File.Create(path))
        using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
        {
            await WriteManifestAsync(archive, createdAt, cancellationToken);
            await AddDatabaseAsync(archive, cancellationToken);
            await AddUploadsAsync(archive, cancellationToken);
        }

        var info = new FileInfo(path);
        return new BackupResultDto(info.Name, info.FullName, info.Length, createdAt);
    }

    public Task<IReadOnlyList<BackupFileDto>> ListAsync(CancellationToken cancellationToken)
    {
        var root = Path.GetFullPath(backupOptions.Value.RootPath);

        if (!Directory.Exists(root))
        {
            return Task.FromResult<IReadOnlyList<BackupFileDto>>([]);
        }

        IReadOnlyList<BackupFileDto> result = Directory
            .EnumerateFiles(root, "lesson-runner-backup-*.zip")
            .Select(path => new FileInfo(path))
            .OrderByDescending(file => file.CreationTimeUtc)
            .Select(file => new BackupFileDto(
                file.Name,
                file.Length,
                new DateTimeOffset(file.CreationTimeUtc, TimeSpan.Zero)))
            .ToList();

        return Task.FromResult(result);
    }

    private async Task WriteManifestAsync(ZipArchive archive, DateTimeOffset createdAt, CancellationToken cancellationToken)
    {
        var entry = archive.CreateEntry("manifest.json", CompressionLevel.Optimal);
        await using var stream = entry.Open();
        await JsonSerializer.SerializeAsync(
            stream,
            new
            {
                createdAt,
                databaseProvider = dbContext.Database.ProviderName,
                databaseBackup = dbContext.Database.IsSqlite() ? "sqlite-file" : "external-pg-dump-required",
                uploadsIncluded = Directory.Exists(fileStorageOptions.Value.RootPath)
            },
            cancellationToken: cancellationToken);
    }

    private async Task AddDatabaseAsync(ZipArchive archive, CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsSqlite())
        {
            return;
        }

        var connectionString = dbContext.Database.GetConnectionString()
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? string.Empty;
        var builder = new SqliteConnectionStringBuilder(connectionString);
        var dataSource = builder.DataSource;

        if (string.IsNullOrWhiteSpace(dataSource) || dataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var databasePath = Path.GetFullPath(dataSource);

        if (!File.Exists(databasePath))
        {
            return;
        }

        // Bez tego kopiujemy sam plik .db, a najświeższe zapisy mogą jeszcze siedzieć
        // w dzienniku -wal (którego do archiwum nie bierzemy) - kopia byłaby niepełna.
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA wal_checkpoint(TRUNCATE);", cancellationToken);

        var entry = archive.CreateEntry($"database/{Path.GetFileName(databasePath)}", CompressionLevel.Optimal);
        await using var entryStream = entry.Open();
        await using var databaseStream = File.Open(databasePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        await databaseStream.CopyToAsync(entryStream, cancellationToken);
    }

    private async Task AddUploadsAsync(ZipArchive archive, CancellationToken cancellationToken)
    {
        var root = Path.GetFullPath(fileStorageOptions.Value.RootPath);

        if (!Directory.Exists(root))
        {
            return;
        }

        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(root, file).Replace('\\', '/');
            var entry = archive.CreateEntry($"uploads/{relative}", CompressionLevel.Optimal);
            await using var entryStream = entry.Open();
            await using var source = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            await source.CopyToAsync(entryStream, cancellationToken);
        }
    }
}
