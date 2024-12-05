using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class WebhookSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StripeWebookId",
                table: "Settings",
                newName: "StripeWebhookSecret");

            migrationBuilder.AddColumn<string>(
                name: "StripeWebhookId",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeWebhookId",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "StripeWebhookSecret",
                table: "Settings",
                newName: "StripeWebookId");
        }
    }
}
