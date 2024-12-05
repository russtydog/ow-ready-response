using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StripeProductOrganisation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "StripeProducts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionPlan",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StripeProducts_OrganisationId",
                table: "StripeProducts",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_StripeProducts_Organisations_OrganisationId",
                table: "StripeProducts",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StripeProducts_Organisations_OrganisationId",
                table: "StripeProducts");

            migrationBuilder.DropIndex(
                name: "IX_StripeProducts_OrganisationId",
                table: "StripeProducts");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "StripeProducts");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "Organisations");
        }
    }
}
