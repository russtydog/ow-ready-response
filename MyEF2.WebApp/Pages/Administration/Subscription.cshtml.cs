using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;
using Stripe;

namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]
    public class SubscriptionModel : PageModel
    {
        private readonly OrganisationService _organisationService;
        private readonly SettingService _settingService;
        private readonly UserService _userService;
        private readonly StripeProductService _stripeProductService;

        public SubscriptionModel(OrganisationService organisationService, SettingService settingService,UserService userService,StripeProductService stripeProductService)
        {
            _organisationService = organisationService;
            _settingService = settingService;
            _userService = userService;
            _stripeProductService = stripeProductService;
        }

        [BindProperty]
        public Subscription MyStripeSubscription { get; set; }
        [BindProperty]
        public Customer MyStripeCustomer { get; set; }
        [BindProperty]
        public string Created { get; set; }
        [BindProperty]
        public string CurrentPeriodEnd { get; set; }
        [BindProperty]
        public string Price { get; set; }
        [BindProperty]
        public StripeProduct StripeProduct { get; set; }
        [BindProperty]
        public string StripeCustomerPortalUrl{get;set;}
        public Setting Setting { get; set; }
        public List<User> OrgAdmins { get; set; }
        [BindProperty]
        public string UpdateBillerName{get;set;}
        [BindProperty]
        public string UpdateBillerEmail{get;set;}

        public async Task<IActionResult> OnGetAsync()
        {
            var ThisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if(ThisUser.IsOrgAdmin!=true)
            {
                return RedirectToPage("/Dasboard");
            }

            Setting=_settingService.GetSettings();

            Organisation organisation = _organisationService.GetOrganisation(ThisUser.Organisation.Id);

            OrgAdmins = _userService.GetUsersForOrganisation(organisation.Id.ToString());

            if(string.IsNullOrEmpty(organisation.StripeSubscriptionId))
            {
                return RedirectToPage("/Administration/Organisation"); 
            }

            var stripeProduct = _stripeProductService.GetStripeProduct(Guid.Parse(organisation.SubscriptionPlan),User.Identity.Name);

            StripeCustomerPortalUrl=Setting.StripeCustomerPortalUrl;

            if (organisation.StripeSubscriptionId != null)
            {
                StripeProduct=stripeProduct;
                StripeConfiguration.ApiKey = Encryption.Decrypt(Setting.StripeSecretKey);
                var service = new SubscriptionService();
                MyStripeSubscription = service.Get(Encryption.Decrypt(organisation.StripeSubscriptionId));

                var customerService = new CustomerService();
                MyStripeCustomer = customerService.Get(organisation.StripeCustomerId);

                Console.WriteLine(JsonSerializer.Serialize(MyStripeSubscription));
                Created = new MyTime(_userService).ConvertUTCToLocalTime(MyStripeSubscription.Created, ThisUser.TimeZone).ToString("dd/MM/yyyy hh:mm:ss tt");
                CurrentPeriodEnd = new MyTime(_userService).ConvertUTCToLocalTime(MyStripeSubscription.CurrentPeriodEnd, ThisUser.TimeZone).ToString("dd/MM/yyyy hh:mm:ss tt");
                Price = "$" + (Convert.ToDecimal(MyStripeSubscription.Items.Data[0].Price.UnitAmount) / 100).ToString("N2");
                return Page();
            }
            else
            {
                // Response.Redirect("/Administration/Stripe/Products/Plans");
                return RedirectToPage("/Dashboard");
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var action = Request.Form["action"];

            if (string.Equals(action, "changebiller", StringComparison.OrdinalIgnoreCase))
            {
                var user = _userService.GetUserByAuthId(User.Identity.Name);
                var settings = _settingService.GetSettings();
                StripeConfiguration.ApiKey = Encryption.Decrypt(settings.StripeSecretKey);
                var customerService = new CustomerService();
                var updateOptions = new CustomerUpdateOptions
                {
                    Name = UpdateBillerName,
                    Email = UpdateBillerEmail
                };
                customerService.Update(user.Organisation.StripeCustomerId, updateOptions);

            }
            return RedirectToPage("/Administration/Subscription");
        }
    }
}
