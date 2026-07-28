using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Znacznik ważności sesji (security stamp) na koncie użytkownika.
    /// Trafia do tokenu JWT jako claim `sst` i jest porównywany z bazą przy każdym żądaniu,
    /// dzięki czemu dezaktywacja konta oraz zmiana/reset hasła unieważniają aktywne sesje
    /// natychmiast, a nie dopiero po wygaśnięciu tokenu.
    ///
    /// Istniejące konta dostają pustą wartość - takich sesji celowo nie wylogowujemy w trakcie
    /// dnia. Znacznik uzupełni się przy najbliższej zmianie hasła.
    ///
    /// Migracja napisana ręcznie. Atrybuty [DbContext] i [Migration] oraz model docelowy
    /// siedzą - tak jak w migracjach generowanych przez `dotnet ef` - w pliku
    /// `20260727120000_AddUserSecurityStamp.Designer.cs`. Bez atrybutu [DbContext] EF w ogóle
    /// nie zalicza klasy do migracji tego kontekstu i po cichu ją pomija.
    /// Snapshot `AppDbContextModelSnapshot.cs` został zaktualizowany razem z nią, więc kolejne
    /// `dotnet ef migrations add` policzą różnice poprawnie.
    /// </summary>
    public partial class AddUserSecurityStamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Users",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Users");
        }
    }
}
