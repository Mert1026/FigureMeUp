using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FigureMeUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AnotherColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LikedByUsersIds",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "LikedByUsersIds",
                table: "Figures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikedByUsersIds",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "LikedByUsersIds",
                table: "Figures");
        }
    }
}
