using MyEF2.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyEF2.DAL.Entities;
using MyEF2.DAL.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace MyEF2.DAL.Services
{
    public class EmailService
    {
        private readonly SettingService _settingService;
        private readonly EmailLogService _emailLogService;
        private readonly DatabaseContext _dbContext;
        private readonly NotificationTemplateService _notificationTemplateService;

        public EmailService() { }
        
        public EmailService(SettingService settingService,DatabaseContext databaseContext,EmailLogService emailLogService,NotificationTemplateService notificationTemplateService)
        {
            _settingService = settingService;
            _dbContext = databaseContext;
            _emailLogService = emailLogService;
            _notificationTemplateService = notificationTemplateService;
        }
        

        
        public void SendEmail(Email email)
        {
            Setting setting = _settingService.GetSettings();  //_settingService.GetSettings();
            
            var smtpClient = new SmtpClient(setting.SMTPServer)
            {
                Port = (int)setting.SMTPPort, // Port number
                Credentials = new NetworkCredential(setting.SMTPUsername, Encryption.Decrypt(setting.SMTPPassword)),
                EnableSsl = setting.SMTPSSL, // Use SSL for secure email
            };


            string strEmailTemplateHtml = setting.DefaultNotificationTemplate.TemplateHTML;
            if (email.UseNotificationTemplate)
            {
                var notificationTemplates = _notificationTemplateService.GetAll();
                var notificationTemplate = notificationTemplates.Where(x => x.IsEmailNotificationTemplate == true).FirstOrDefault();
                if (notificationTemplate != null)
                {
                    strEmailTemplateHtml = notificationTemplate.TemplateHTML;
                }
            }

            strEmailTemplateHtml=strEmailTemplateHtml.Replace("[[FirstName]]",email.FirstName);
            strEmailTemplateHtml=strEmailTemplateHtml.Replace("[[LastName]]", email.LastName);
            strEmailTemplateHtml=strEmailTemplateHtml.Replace("[[Email]]",email.Recipients[0].ToString());
            strEmailTemplateHtml = strEmailTemplateHtml.Replace("[[MessageBody]]", email.Body);
            strEmailTemplateHtml = strEmailTemplateHtml.Replace("[[ActionButton]]", email.ActionButton);
            strEmailTemplateHtml = strEmailTemplateHtml.Replace("[[ActionButtonLabel]]", email.ActionButtonLabel);

            // Create a MailMessage
            var mailMessage = new MailMessage
            {
                From = new MailAddress(setting.SMTPSenderEmail,setting.SMTPSenderName),
                Subject = email.Subject,
                Body = strEmailTemplateHtml,
            };

            foreach(var mail in email.Recipients)
            {
                mailMessage.To.Add(mail);
            }
            mailMessage.IsBodyHtml = true;
            // Send the email
            smtpClient.Send(mailMessage);

            EmailLog emailLog = new EmailLog();
            emailLog.Subject=email.Subject;
            emailLog.SenderEmail = setting.SMTPSenderEmail;
            emailLog.Body=email.Body;
            emailLog.SenderName = setting.SMTPSenderName;
            emailLog.Created = DateTime.UtcNow;
            emailLog.Recipient = email.Recipients[0];

            //EmailLogService emailLogService = new EmailLogService();
            var resp = _emailLogService.Create(emailLog);
            //Create(emailLog);
        }

        public EmailLog Create(EmailLog emailLog)
        {
            _dbContext.Add(emailLog);
            _dbContext.SaveChanges();
            return emailLog;
        }
    }
}
