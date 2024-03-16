using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SummaBook.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addProductToShoppingCartModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_ProductId",
                table: "ShoppingCart",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Products_ProductId",
                table: "ShoppingCart",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Products_ProductId",
                table: "ShoppingCart");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCart_ProductId",
                table: "ShoppingCart");
        }
    }
}
