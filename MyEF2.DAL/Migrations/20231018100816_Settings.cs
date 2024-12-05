using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMTPServer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMTPUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMTPPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMTPPort = table.Column<int>(type: "int", nullable: true),
                    SMTPSSL = table.Column<bool>(type: "bit", nullable: false),
                    SMTPSenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMTPSenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO Settings (Id, CompanyName,SMTPServer, SMTPUsername, SMTPPassword, SMTPPort,SMTPSSL,SMTPSenderName,SMTPSenderEmail) SELECT NewID(),'ACME Ltd','mail.smtp2go.com','dev.russelldoig','9E5r9of1hivNlHJAiVMb1GuGxlDU0wXn',2525,0,'Developer','dev@russelldoig.com'");        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}
