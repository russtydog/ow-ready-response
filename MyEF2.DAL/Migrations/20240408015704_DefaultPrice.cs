using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DefaultPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeProductPrices_StripeProducts_StripeProductId",
                table: "StripeProductPrices");

            migrationBuilder.AlterColumn<Guid>(
                name: "StripeProductId",
                table: "StripeProductPrices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "StripeProductPrices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_StripeProductPrices_StripeProducts_StripeProductId",
                table: "StripeProductPrices",
                column: "StripeProductId",
                principalTable: "StripeProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeProductPrices_StripeProducts_StripeProductId",
                table: "StripeProductPrices");

            migrationBuilder.DropColumn(
                name: "Default",
                table: "StripeProductPrices");

            migrationBuilder.AlterColumn<Guid>(
                name: "StripeProductId",
                table: "StripeProductPrices",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeProductPrices_StripeProducts_StripeProductId",
                table: "StripeProductPrices",
                column: "StripeProductId",
                principalTable: "StripeProducts",
                principalColumn: "Id");
        }
    }
}
