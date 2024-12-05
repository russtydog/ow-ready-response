using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AskAIAPIFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AskAIAPI",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AskAIAPIKey",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AskAIAPI",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "AskAIAPIKey",
                table: "Organisations");
        }
    }
}
