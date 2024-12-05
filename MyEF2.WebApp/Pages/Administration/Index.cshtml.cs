using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Stripe;

namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly OrganisationService _organisationService;
        private readonly UserService _userService;
        private readonly NotificationTemplateService _notificationTemplateService;
        [BindProperty]
        public DAL.Entities.Setting Setting { get; set; }
        [BindProperty]
        public IFormFile UploadFile { get; set; }
        [BindProperty]
        public IFormFile UploadFileFavIcon { get; set; }
        [BindProperty]
        public List<NotificationTemplate> NotificationTemplates { get; set; }
        [BindProperty]
        public List<Organisation> Organisations { get; set; }
        public IndexModel(SettingService settingService,UserService userService,NotificationTemplateService notificationTemplateService,OrganisationService organisationService)
        {
            _settingService = settingService;
            _userService = userService;
            _notificationTemplateService = notificationTemplateService;
            _organisationService = organisationService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if(!thisUser.IsAdmin)
            {
                return RedirectToPage("../Dashboard");
            }
            NotificationTemplates = _notificationTemplateService.GetAll();
            Organisations=_organisationService.GetOrganisationList();

            Setting = _settingService.GetSettings(thisUser.Email);
            Setting.SMTPPassword = Setting.SMTPPassword==""?"":Encryption.Decrypt(Setting.SMTPPassword);
            Setting.OpenAIKey = Setting.OpenAIKey == null ? "" : Encryption.Decrypt(Setting.OpenAIKey);
            Setting.StripeSecretKey = Setting.StripeSecretKey==null?"": Encryption.Decrypt(Setting.StripeSecretKey);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Logo handler
            if (UploadFile != null && UploadFile.Length > 0)
            {
                string fileName = "Logo" + Path.GetExtension(UploadFile.FileName);
                string filePath = Path.Combine("wwwroot/dist/img/", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
                Setting.Logo = fileName;
            }
            //FavIcon Logo handler
            if (UploadFileFavIcon != null && UploadFileFavIcon.Length > 0)
            {
                string fileName = "favicon.ico";
                string filePath = Path.Combine("wwwroot/", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    UploadFileFavIcon.CopyTo(stream);
                }
                Setting.FavIcon = fileName;
            }

            NotificationTemplate notificationTemplate = _notificationTemplateService.GetById(Setting.DefaultNotificationTemplate.Id);
            Setting.DefaultNotificationTemplate = notificationTemplate;

            Setting.StripeSecretKey = string.IsNullOrEmpty(Setting.StripeSecretKey) ? "" : Encryption.Encrypt(Setting.StripeSecretKey);
            Setting.OpenAIKey = string.IsNullOrEmpty(Setting.OpenAIKey) ? "" : Encryption.Encrypt(Setting.OpenAIKey);

            if (string.IsNullOrEmpty(Setting.StripeWebhookId)&&Setting.OrganisationsRequireSubscription)
            {
                StripeConfiguration.ApiKey = Setting.StripeSecretKey;

                var options = new WebhookEndpointCreateOptions
                {
                    Url = $"{Request.Scheme}://{Request.Host}" + "/api/StripeWebhook",
                    //Url = "https://mysite.com/api/StripeWebhook",
                    EnabledEvents = new List<string>
                    {
                        "invoice.paid",
                        "invoice.payment_failed",
                        // Add other event types here as needed
                    },
                    // Optionally, specify a description or metadata for easier management
                    Description = Setting.ApplicationName + " webhook for invoice events",
                };

                var service = new WebhookEndpointService();
                try
                {
                    var webhookEndpoint = await service.CreateAsync(options);
                    Setting.StripeWebhookId = webhookEndpoint.Id;
                    Setting.StripeWebhookSecret = webhookEndpoint.Secret;
                }
                catch (Exception ex)
                {
                    Setting.StripeWebhookId = "";
                    Setting.StripeWebhookSecret = "";
                }
            }


            ModelState.Remove("UploadFile");
            ModelState.Remove("UploadFileFavIcon");
            ModelState.Remove("Setting.DefaultNotificationTemplate.TemplateName");
            ModelState.Remove("Setting.StripeWebookId");
            if (ModelState.IsValid)
            {
                
                var resp = _settingService.UpdateSettings(Setting,User.Identity.Name);
                return RedirectToPage("../Dashboard");
            }
            return Page();
        }
    }
}
