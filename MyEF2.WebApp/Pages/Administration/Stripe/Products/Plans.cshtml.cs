using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Stripe;

namespace MyEF2.WebApp.Pages.Administration.Stripe.Products
{
    [Authorize]
    public class PlansModel : PageModel
    {
        private readonly StripeProductService _stripeProductService;
        private readonly UserService _userService;
        private readonly SettingService _settingService;
        private readonly OrganisationService _organisationService;
        
        public PlansModel(StripeProductService stripeProductService, UserService userService,SettingService settingService,OrganisationService organisationService)
        {
            _stripeProductService = stripeProductService;
            _userService = userService;
            _settingService = settingService;
            _organisationService = organisationService;
        }
		[BindProperty(SupportsGet = true)]
		public Guid? Id { get; set; }
        public List<StripeProduct> StripeProducts { get; set; }
        [BindProperty]
        public string SubscriptionPlan { get; set; }
		public void OnGet()
        {
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsOrgAdmin)
            {
                RedirectToPage("/Index");
            }
            StripeProducts = _stripeProductService.GetStripeProducts().OrderBy(p => p.Name).ToList();

            SubscriptionPlan=thisUser.Organisation.SubscriptionPlan;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var action = Request.Form["action"];

            if (string.Equals(action, "cancel", StringComparison.OrdinalIgnoreCase))
            {
                User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
                if (!thisUser.IsOrgAdmin)
                {
                    RedirectToPage("/Index");
                }
                thisUser.Organisation.SubscriptionPlan = SubscriptionPlan;

                Setting setting = _settingService.GetSettings();

                StripeConfiguration.ApiKey = Encryption.Decrypt(setting.StripeSecretKey);

                try
                {
                    var cancelService = new SubscriptionService();
                    cancelService.Cancel(Encryption.Decrypt(thisUser.Organisation.StripeSubscriptionId));
                }
                catch(Exception ex)
                {
                    //something went wrong cancelling, may have already cancelled
                }
                //get stripeproducts to get the list of stripeprices and then filter on the prices by default=true
                var products = _stripeProductService.GetStripeProducts();

                Organisation organisation = _organisationService.GetOrganisation(thisUser.Organisation.Id);
                organisation.SubscriptionPlan = products.FirstOrDefault().Id.ToString();
                organisation.StripeSubscriptionId = null;
                _organisationService.UpdateOrganisations(organisation, User.Identity.Name);

            }
            return RedirectToPage("/Dashboard");
        }   
    }
}
