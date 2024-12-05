using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StripeSubscriptionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripePlanId",
                table: "StripeSubscriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "StripeSubscriptions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripePlanId",
                table: "StripeSubscriptions");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "StripeSubscriptions");
        }
    }
}
