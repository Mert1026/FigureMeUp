using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FigureMeUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class KeysChange_pt1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Figures_AspNetUsers_OwnerId",
                table: "Figures");

            migrationBuilder.DropForeignKey(
                name: "FK_Figures_Rarities_RarityId",
                table: "Figures");

            migrationBuilder.DropForeignKey(
                name: "FK_Hashtags_Figures_FigureId",
                table: "Hashtags");

            migrationBuilder.DropForeignKey(
                name: "FK_Hashtags_Posts_PostId",
                table: "Hashtags");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_PublisherId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFigures_Figures_FigureId",
                table: "UserFigures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rarities",
                table: "Rarities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hashtags",
                table: "Hashtags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Figures",
                table: "Figures");

            migrationBuilder.RenameTable(
                name: "Rarities",
                newName: "Rarity");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "Hashtags",
                newName: "Hashtag");

            migrationBuilder.RenameTable(
                name: "Figures",
                newName: "Figure");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_PublisherId",
                table: "Post",
                newName: "IX_Post_PublisherId");

            migrationBuilder.RenameIndex(
                name: "IX_Hashtags_PostId",
                table: "Hashtag",
                newName: "IX_Hashtag_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Hashtags_FigureId",
                table: "Hashtag",
                newName: "IX_Hashtag_FigureId");

            migrationBuilder.RenameIndex(
                name: "IX_Figures_RarityId",
                table: "Figure",
                newName: "IX_Figure_RarityId");

            migrationBuilder.RenameIndex(
                name: "IX_Figures_OwnerId",
                table: "Figure",
                newName: "IX_Figure_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rarity",
                table: "Rarity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hashtag",
                table: "Hashtag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Figure",
                table: "Figure",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Figure_AspNetUsers_OwnerId",
                table: "Figure",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Figure_Rarity_RarityId",
                table: "Figure",
                column: "RarityId",
                principalTable: "Rarity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hashtag_Figure_FigureId",
                table: "Hashtag",
                column: "FigureId",
                principalTable: "Figure",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hashtag_Post_PostId",
                table: "Hashtag",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_PublisherId",
                table: "Post",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFigures_Figure_FigureId",
                table: "UserFigures",
                column: "FigureId",
                principalTable: "Figure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Figure_AspNetUsers_OwnerId",
                table: "Figure");

            migrationBuilder.DropForeignKey(
                name: "FK_Figure_Rarity_RarityId",
                table: "Figure");

            migrationBuilder.DropForeignKey(
                name: "FK_Hashtag_Figure_FigureId",
                table: "Hashtag");

            migrationBuilder.DropForeignKey(
                name: "FK_Hashtag_Post_PostId",
                table: "Hashtag");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_PublisherId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFigures_Figure_FigureId",
                table: "UserFigures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rarity",
                table: "Rarity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Hashtag",
                table: "Hashtag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Figure",
                table: "Figure");

            migrationBuilder.RenameTable(
                name: "Rarity",
                newName: "Rarities");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "Hashtag",
                newName: "Hashtags");

            migrationBuilder.RenameTable(
                name: "Figure",
                newName: "Figures");

            migrationBuilder.RenameIndex(
                name: "IX_Post_PublisherId",
                table: "Posts",
                newName: "IX_Posts_PublisherId");

            migrationBuilder.RenameIndex(
                name: "IX_Hashtag_PostId",
                table: "Hashtags",
                newName: "IX_Hashtags_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Hashtag_FigureId",
                table: "Hashtags",
                newName: "IX_Hashtags_FigureId");

            migrationBuilder.RenameIndex(
                name: "IX_Figure_RarityId",
                table: "Figures",
                newName: "IX_Figures_RarityId");

            migrationBuilder.RenameIndex(
                name: "IX_Figure_OwnerId",
                table: "Figures",
                newName: "IX_Figures_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rarities",
                table: "Rarities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Hashtags",
                table: "Hashtags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Figures",
                table: "Figures",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Figures_AspNetUsers_OwnerId",
                table: "Figures",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Figures_Rarities_RarityId",
                table: "Figures",
                column: "RarityId",
                principalTable: "Rarities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hashtags_Figures_FigureId",
                table: "Hashtags",
                column: "FigureId",
                principalTable: "Figures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hashtags_Posts_PostId",
                table: "Hashtags",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_PublisherId",
                table: "Posts",
                column: "PublisherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFigures_Figures_FigureId",
                table: "UserFigures",
                column: "FigureId",
                principalTable: "Figures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
