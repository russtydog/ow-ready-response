using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SettingsTermsOfService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TermsOfService",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsOfService",
                table: "Settings");
        }
    }
}
