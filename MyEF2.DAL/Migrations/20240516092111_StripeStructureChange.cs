using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StripeStructureChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StripeProductPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "StripeProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "StripeProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentLink",
                table: "StripeProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanId",
                table: "StripeProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "StripeProducts");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "StripeProducts");

            migrationBuilder.DropColumn(
                name: "PaymentLink",
                table: "StripeProducts");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "StripeProducts");

            migrationBuilder.CreateTable(
                name: "StripeProductPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StripeProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeProductPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StripeProductPrices_StripeProducts_StripeProductId",
                        column: x => x.StripeProductId,
                        principalTable: "StripeProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StripeProductPrices_StripeProductId",
                table: "StripeProductPrices",
                column: "StripeProductId");
        }
    }
}
