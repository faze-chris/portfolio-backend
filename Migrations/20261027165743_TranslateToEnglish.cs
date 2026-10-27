using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace portfolio_backend.Migrations
{
    /// <inheritdoc />
    public partial class TranslateToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Titel",
                table: "Projects",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Datum",
                table: "Projects",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "Categorie",
                table: "Projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "Beschrijving",
                table: "Projects",
                newName: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Projects",
                newName: "Titel");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Projects",
                newName: "Categorie");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Projects",
                newName: "Datum");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Projects",
                newName: "Beschrijving");
        }
    }
}
