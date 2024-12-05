using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MenuDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			//create menu items if they don't already exist.

			//check if Dashboard Menu exists
			string checkAndInsertSQL = @"
                IF NOT EXISTS(SELECT 1 FROM Menus WHERE [Name] = 'Dashboard')
                BEGIN
                    INSERT INTO Menus (Id,Name, Url, [Order],Icon,MenuId) VALUES (NEWID(),'Dashboard', '/Dashboard', 1, 'fas fa-tachometer-alt',null)
                END";

			migrationBuilder.Sql(checkAndInsertSQL);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
