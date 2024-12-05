using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class OrganisationSSO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableSSO",
                table: "Organisations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EntityID",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoginURL",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SigningCertificate",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableSSO",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "EntityID",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "LoginURL",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "SigningCertificate",
                table: "Organisations");
        }
    }
}
