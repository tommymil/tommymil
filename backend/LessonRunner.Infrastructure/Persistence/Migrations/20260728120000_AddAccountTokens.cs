using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonRunner.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Tokeny jednorazowe do ustawienia hasła: reset i zaproszenie.
    ///
    /// Bez tej tabeli każde hasło ustawiał ręcznie administrator — przy trzydziestu rodzicach
    /// jest to etat, a przy okazji hasło wędruje kanałem, którego nikt nie kontroluje.
    ///
    /// `TokenHash` to skrót SHA-256, nie token. Kolumna jest unikalna, bo skrót jest
    /// jednocześnie kluczem wyszukiwania. Tokenu w postaci jawnej nie ma nigdzie poza wysłanym
    /// e-mailem — wyciek kopii bazy nie może oznaczać przejęcia kont.
    ///
    /// Tabela bez klucza obcego do `Users`: usunięcie konta nie powinno wywracać zapisu o tym,
    /// że token kiedyś wydano, a sprzątanie i tak idzie po `UsedAt` oraz `ExpiresAt`.
    ///
    /// Migracja napisana ręcznie; model docelowy w pliku *.Designer.cs.
    /// </summary>
    public partial class AddAccountTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TokenHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Purpose = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    IssuedByUserId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountTokens_TokenHash",
                table: "AccountTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountTokens_UserId_UsedAt",
                table: "AccountTokens",
                columns: new[] { "UserId", "UsedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AccountTokens");
        }
    }
}
