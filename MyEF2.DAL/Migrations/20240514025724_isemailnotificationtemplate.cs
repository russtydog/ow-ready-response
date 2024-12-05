using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class isemailnotificationtemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEmailNotificationTemplate",
                table: "NotificationTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("INSERT INTO NotificationTemplates (Id, TemplateName, TemplateHTML, TemplateJson, IsEmailNotificationTemplate) select top 1 newid(), 'System Notification', TemplateHTML, TemplateJson, 1 from NotificationTemplates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailNotificationTemplate",
                table: "NotificationTemplates");
        }
    }
}
