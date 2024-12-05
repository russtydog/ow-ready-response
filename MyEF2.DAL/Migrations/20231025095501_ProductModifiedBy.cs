using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ProductModifiedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
			migrationBuilder.Sql(@"UPDATE Products
                Set ModifiedById = (select top 1 Id from Users where OrganisationId=Products.OrganisationId)
                ");

			migrationBuilder.CreateIndex(
                name: "IX_Products_ModifiedById",
                table: "Products",
                column: "ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_ModifiedById",
                table: "Products",
                column: "ModifiedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_ModifiedById",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ModifiedById",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Products");
        }
    }
}
