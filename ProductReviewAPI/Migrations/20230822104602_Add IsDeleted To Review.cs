using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductReviewAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SellerProduct_Sellers_SellerId",
                table: "SellerProduct");

            migrationBuilder.DropIndex(
                name: "IX_SellerProduct_SellerId",
                table: "SellerProduct");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_SellerProduct_SellerId",
                table: "SellerProduct",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SellerProduct_Sellers_SellerId",
                table: "SellerProduct",
                column: "SellerId",
                principalTable: "Sellers",
                principalColumn: "SellerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
