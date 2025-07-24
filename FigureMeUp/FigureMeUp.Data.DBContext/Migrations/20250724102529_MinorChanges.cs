using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FigureMeUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class MinorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hashtags",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Rarities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Figures",
                newName: "LastChanged");

            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Hashtags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hashtags_PostId",
                table: "Hashtags",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hashtags_Posts_PostId",
                table: "Hashtags",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hashtags_Posts_PostId",
                table: "Hashtags");

            migrationBuilder.DropIndex(
                name: "IX_Hashtags_PostId",
                table: "Hashtags");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Hashtags");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Rarities",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "LastChanged",
                table: "Figures",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Hashtags",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
