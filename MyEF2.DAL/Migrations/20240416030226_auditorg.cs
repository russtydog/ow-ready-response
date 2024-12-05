using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class auditorg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "Audits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Audits_OrganisationId",
                table: "Audits",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audits_Organisations_OrganisationId",
                table: "Audits",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audits_Organisations_OrganisationId",
                table: "Audits");

            migrationBuilder.DropIndex(
                name: "IX_Audits_OrganisationId",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Audits");
        }
    }
}
