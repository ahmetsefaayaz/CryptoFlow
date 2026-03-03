using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OrderServiceUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coins_CoinId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CoinId",
                table: "Orders",
                newName: "TargetCoinId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CoinId",
                table: "Orders",
                newName: "IX_Orders_TargetCoinId");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentCoinId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentCoinId",
                table: "Orders",
                column: "PaymentCoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Coins_PaymentCoinId",
                table: "Orders",
                column: "PaymentCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Coins_TargetCoinId",
                table: "Orders",
                column: "TargetCoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coins_PaymentCoinId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Coins_TargetCoinId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentCoinId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentCoinId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TargetCoinId",
                table: "Orders",
                newName: "CoinId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_TargetCoinId",
                table: "Orders",
                newName: "IX_Orders_CoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Coins_CoinId",
                table: "Orders",
                column: "CoinId",
                principalTable: "Coins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
