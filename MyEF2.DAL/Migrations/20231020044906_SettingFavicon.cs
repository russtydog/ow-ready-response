using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SettingFavicon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FavIcon",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql("Update Settings SET FavIcon='favicon.png'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavIcon",
                table: "Settings");
        }
    }
}
