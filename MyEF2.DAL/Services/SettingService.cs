using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;

namespace MyEF2.DAL.Services
{
    public class SettingService
    {
        private readonly DatabaseContext _dbContext;
        public SettingService() { }
        public SettingService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
            

        //Not Audit as it's used to get settings for a purpose
        public Setting GetSettings()
        {
            Setting settings = _dbContext.Settings.Include(x => x.DefaultNotificationTemplate).FirstOrDefault();
            return settings;
        }
        //User is admin viewing settings for Audit
        public Setting GetSettings(string UserName) {
            Setting settings = _dbContext.Settings.Include(x => x.DefaultNotificationTemplate).FirstOrDefault();

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(settings, null, "Settings", settings.Id.ToString(), null, UserName, "View");

            return settings;
        }
        public Setting UpdateSettings(Setting settings,string UserName)
        {
            Setting originalSettings = _dbContext.Settings.AsNoTracking().Include(x => x.DefaultNotificationTemplate).FirstOrDefault();
            Setting existingSettings = _dbContext.Settings.Include(x=>x.DefaultNotificationTemplate).FirstOrDefault();

            

            existingSettings.ApplicationName=settings.ApplicationName;
            existingSettings.CompanyName=settings.CompanyName;
            existingSettings.Logo = settings.Logo;
            existingSettings.SMTPServer=settings.SMTPServer;
            existingSettings.SMTPUsername = settings.SMTPUsername;
            existingSettings.SMTPPassword= Encryption.Encrypt(settings.SMTPPassword);
            existingSettings.SMTPPort = settings.SMTPPort;
            existingSettings.SMTPSSL = settings.SMTPSSL;
            existingSettings.SMTPSenderName = settings.SMTPSenderName;
            existingSettings.SMTPSenderEmail=settings.SMTPSenderEmail;
            existingSettings.FavIcon = settings.FavIcon;
            existingSettings.UseOrganisations=settings.UseOrganisations;
            existingSettings.EnableSSO = settings.EnableSSO;
            existingSettings.EnableAutoRegistration=settings.EnableAutoRegistration;
            existingSettings.EntityID=settings.EntityID;
            existingSettings.LoginURL=settings.LoginURL;
            existingSettings.SigningCertificate = settings.SigningCertificate;
            existingSettings.APIUrl=settings.APIUrl;
            existingSettings.CompanyWebsite = settings.CompanyWebsite;
            existingSettings.EnableRegistration = settings.EnableRegistration;
            existingSettings.ActiveItemBackgroundColor=settings.ActiveItemBackgroundColor;
            existingSettings.ActiveItemBackgroundColorHover=settings.ActiveItemBackgroundColorHover;
            existingSettings.ItemTextColor=settings.ItemTextColor;
            existingSettings.DefaultOrganisationId = settings.DefaultOrganisationId;
            existingSettings.EnableOrganisationSSO = settings.EnableOrganisationSSO;
            existingSettings.StripeSecretKey = settings.StripeSecretKey;
            existingSettings.StripeWebhookId = settings.StripeWebhookId;
            existingSettings.StripeWebhookSecret = settings.StripeWebhookSecret;
            existingSettings.OpenAIKey = settings.OpenAIKey;
            existingSettings.UseFrontend = settings.UseFrontend;
            existingSettings.TermsOfService = settings.TermsOfService;
            existingSettings.OrganisationsRequireSubscription = settings.OrganisationsRequireSubscription;
            existingSettings.StripeCustomerPortalUrl = settings.StripeCustomerPortalUrl;
            existingSettings.StripeUsagePriceId=settings.StripeUsagePriceId;
            existingSettings.StripeMeterEventName=settings.StripeMeterEventName;
            existingSettings.SlickTrackerUrl=settings.SlickTrackerUrl;
            // End Stadard Fields and Properties



            var notificationTemplate = _dbContext.NotificationTemplates.FirstOrDefault(x => x.Id == settings.DefaultNotificationTemplate.Id);
            existingSettings.DefaultNotificationTemplate = notificationTemplate;
            

            _dbContext.Update(existingSettings);
            _dbContext.SaveChanges();

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(existingSettings, originalSettings, "Settings", existingSettings.Id.ToString(), null, UserName, "Update");

            return settings;

        }
        
    }
}
