using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FigureMeUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingRarities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rarities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Common" },
                    { 2, "Rare" },
                    { 3, "Epic" },
                    { 4, "Legendary" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rarities",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
