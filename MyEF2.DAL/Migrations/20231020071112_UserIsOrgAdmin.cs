using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UserIsOrgAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOrgAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql("Update Users SET IsOrgAdmin=0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOrgAdmin",
                table: "Users");
        }
    }
}
