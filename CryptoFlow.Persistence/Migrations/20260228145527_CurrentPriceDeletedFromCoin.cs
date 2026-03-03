using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CurrentPriceDeletedFromCoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "Coins");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentPrice",
                table: "Coins",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
