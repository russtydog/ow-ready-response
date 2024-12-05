using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ActiveSubscription",
                table: "Organisations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveSubscription",
                table: "Organisations");
        }
    }
}
