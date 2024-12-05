using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ProductOrganisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("UPDATE Products SET OrganisationId=(select top 1 Id from Organisations)");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrganisationId",
                table: "Products",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Organisations_OrganisationId",
                table: "Products",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Organisations_OrganisationId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrganisationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Products");
        }
    }
}
