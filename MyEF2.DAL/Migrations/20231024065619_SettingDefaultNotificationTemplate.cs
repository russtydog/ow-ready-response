using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyEF2.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SettingDefaultNotificationTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultNotificationTemplateId",
                table: "Settings",
                type: "uniqueidentifier",
                nullable: false,
                 defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(@"
                    declare @id nvarchar(100) = NEWID()

                    insert into NotificationTemplates (Id, TemplateName,TemplateHTML, TemplateJson)
                    select @id,'Default Template',
                    '<!DOCTYPE html><html xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" lang=""en""><head><title></title><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""><meta name=""viewport"" content=""width=device-width,initial-scale=1""><!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]--><style>  *{box-sizing:border-box}body{margin:0;padding:0}a[x-apple-data-detectors]{color:inherit!important;text-decoration:inherit!important}#MessageViewBody a{color:inherit;text-decoration:none}p{line-height:inherit}.desktop_hide,.desktop_hide table{mso-hide:all;display:none;max-height:0;overflow:hidden}.image_block img+div{display:none} @media (max-width:695px){.mobile_hide{display:none}.row-content{width:100%!important}.stack .column{width:100%;display:block}.mobile_hide{min-height:0;max-height:0;max-width:0;overflow:hidden;font-size:0}.desktop_hide,.desktop_hide table{display:table!important;max-height:none!important}}  </style></head><body style=""background-color:#fff;margin:0;padding:0;-webkit-text-size-adjust:none;text-size-adjust:none""><table class=""nl-container"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;background-color:#fff""><tbody><tr><td><table class=""row row-1"" align=""center"" width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0""><tbody><tr><td><table   class=""row-content stack"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;border-radius:0;color:#000;width:675px;margin:0 auto"" width=""675""><tbody><tr><td class=""column column-1"" width=""100%"" style=""mso-table-lspace:0;mso-table-rspace:0;font-weight:400;text-align:left;padding-bottom:5px;padding-top:5px;vertical-align:top;border-top:0;border-right:0;border-bottom:0;border-left:0""><table class=""text_block block-1""   width=""100%"" border=""0"" cellpadding=""10"" cellspacing=""0"" role=""presentation"" style=""mso-table-lspace:0;mso-table-rspace:0;word-break:break-word""><tr><td class=""pad""><div style=""font-family:sans-serif""><div class style=""font-size:14px;font-family:Arial,''Helvetica Neue'',Helvetica,sans-serif;mso-line-height-alt:16.8px;color:#555;line-height:1.2""><p style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">Dear [[FirstName]] ,</p><p   style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">&nbsp;</p><p style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">&nbsp;</p><p style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">[[MessageBody]]&nbsp;</p><p style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">&nbsp;</p><p style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">&nbsp;</p><p style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">Regards,</p><p   style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">&nbsp;</p><p style=""margin:0;font-size:14px;mso-line-height-alt:16.8px"">&nbsp;</p></div></div></td></tr></table></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table><!-- End --></body></html>'
                    ,'{""page"":{""body"":{""container"":{""style"":{""background-color"":""#FFFFFF""}},""content"":{""computedStyle"":{""linkColor"":""#FF819C"",""messageBackgroundColor"":""transparent"",""messageWidth"":""675px""},""style"":{""color"":""#000000"",""font-family"":""Arial, ''Helvetica Neue'', Helvetica, sans-serif""}},""type"":""mailup-bee-page-properties"",""webFonts"":[{""fontFamily"":""''Montserrat'', ''Trebuchet MS'', ''Lucida Grande'', ''Lucida Sans Unicode'', ''Lucida Sans'', Tahoma, sans-serif"",""name"":""Montserrat"",""url"":""https://fonts.googleapis.com/css?family=Montserrat""}]},""description"":""BF-ecommerce-template"",""rows"":[{""container"":{""style"":{""background-color"":""transparent"",""background-image"":""none"",""background-repeat"":""no-repeat"",""background-position"":""top left""}},""content"":{""style"":{""background-color"":""transparent"",""color"":""#000000"",""width"":""500px"",""background-image"":""none"",""background-repeat"":""no-repeat"",""background-position"":""top left"",""border-top"":""0px solid transparent"",""border-right"":""0px solid transparent"",""border-bottom"":""0px solid transparent"",""border-left"":""0px solid transparent"",""border-radius"":""0px""},""computedStyle"":{""rowColStackOnMobile"":true,""rowReverseColStackOnMobile"":false,""verticalAlign"":""top"",""hideContentOnMobile"":false,""hideContentOnDesktop"":false}},""columns"":[{""grid-columns"":12,""modules"":[{""type"":""mailup-bee-newsletter-modules-text"",""descriptor"":{""text"":{""html"":""<div class=\""txtTinyMce-wrapper\"" style=\""font-size: 14px; line-height: 16px;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\""><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">Dear <code spellcheck=\""false\"" data-bee-type=\""mergetag\"" data-bee-code=\""\"" data-bee-name=\""First Name\"">[[FirstName]]</code> ,</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">&nbsp;</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">&nbsp;</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\""><code spellcheck=\""false\"" data-bee-type=\""mergetag\"" data-bee-code=\""\"" data-bee-name=\""Message Body\"">[[MessageBody]]</code>&nbsp;</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">&nbsp;</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">&nbsp;</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">Regards,</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">&nbsp;</p><p style=\""font-size: 14px; line-height: 16px; word-break: break-word;\"" data-mce-style=\""font-size: 14px; line-height: 16px;\"">&nbsp;</p></div>"",""style"":{""color"":""#555555"",""line-height"":""120%"",""font-family"":""inherit""},""computedStyle"":{""linkColor"":""#FF819C""}},""style"":{""padding-top"":""10px"",""padding-right"":""10px"",""padding-bottom"":""10px"",""padding-left"":""10px""},""computedStyle"":{""hideContentOnMobile"":false},""mobileStyle"":{}},""uuid"":""7b1e49d9-28a7-4aca-a88a-dd5da0893276""}],""style"":{""background-color"":""transparent"",""padding-top"":""5px"",""padding-right"":""0px"",""padding-bottom"":""5px"",""padding-left"":""0px"",""border-top"":""0px solid transparent"",""border-right"":""0px solid transparent"",""border-bottom"":""0px solid transparent"",""border-left"":""0px solid transparent""},""uuid"":""004ff0a1-7eba-4961-8452-5e68952a4650""}],""uuid"":""27ee28d8-23c9-424a-b9eb-b93347b836b6""}],""template"":{""name"":""template-base"",""type"":""basic"",""version"":""2.0.0""},""title"":""BF-ecommerce-template""},""comments"":{}}'

                    select id from NotificationTemplates where Id=@id
                    update Settings Set DefaultNotificationTemplateId=@id
                ");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_NotificationTemplates_DefaultNotificationTemplateId",
                table: "Settings",
                column: "DefaultNotificationTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settings_NotificationTemplates_DefaultNotificationTemplateId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "DefaultNotificationTemplateId",
                table: "Settings");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_NotificationTemplates_DefaultNotificationTemplateId",
                table: "Settings",
                column: "DefaultNotificationTemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "Id");
        }
    }
}
