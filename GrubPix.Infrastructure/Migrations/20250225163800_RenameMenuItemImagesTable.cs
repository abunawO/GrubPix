using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrubPix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameMenuItemImagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemImage_MenuItems_MenuItemId",
                table: "MenuItemImage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemImage",
                table: "MenuItemImage");

            migrationBuilder.RenameTable(
                name: "MenuItemImage",
                newName: "MenuItemImages");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemImage_MenuItemId",
                table: "MenuItemImages",
                newName: "IX_MenuItemImages_MenuItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemImages",
                table: "MenuItemImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemImages_MenuItems_MenuItemId",
                table: "MenuItemImages",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemImages_MenuItems_MenuItemId",
                table: "MenuItemImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemImages",
                table: "MenuItemImages");

            migrationBuilder.RenameTable(
                name: "MenuItemImages",
                newName: "MenuItemImage");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemImages_MenuItemId",
                table: "MenuItemImage",
                newName: "IX_MenuItemImage_MenuItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemImage",
                table: "MenuItemImage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemImage_MenuItems_MenuItemId",
                table: "MenuItemImage",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
