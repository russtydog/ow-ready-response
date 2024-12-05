using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]
    public class OrganisationDetailsModel : PageModel
    {
        private readonly UserService _userService;
        private readonly OrganisationService _organisationService;
        private readonly SettingService _settingService;
        private readonly StripeProductService _stripeProductService;

        public OrganisationDetailsModel(UserService userService,OrganisationService organisationService,SettingService settingService,StripeProductService stripeProductService)
        {
            _userService = userService;
            _organisationService = organisationService;
            _settingService = settingService;
            _stripeProductService = stripeProductService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        [BindProperty]
        public Organisation Organisation { get; set; }
        public List<User> Users { get; set; }
        [BindProperty]
        public Setting Settings{get;set;}
        public List<StripeProduct> StripeProducts { get; set; }
        public async Task<ActionResult> OnGetAsync(string? id)
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            if (!user.IsAdmin)
            {
                return RedirectToPage("../Index");
            }
            if (id != null)
            {
                Organisation = _organisationService.GetOrganisation(Guid.Parse(id));
                Users = _userService.GetUsersForOrganisation(Organisation.Id.ToString());
            }

            Settings = _settingService.GetSettings();
            StripeProducts = _stripeProductService.GetStripeProducts();
            return Page();
        }
        public async Task<ActionResult> OnPostAsync()
        {
            
            if (Organisation.Id == Guid.Empty)
            {
                _organisationService.Create(Organisation,User.Identity.Name);
            }
            else
            {
                var existingOrganisation = _organisationService.GetOrganisation(Organisation.Id);
                existingOrganisation.OrganisationName = Organisation.OrganisationName;
                existingOrganisation.ABN = Organisation.ABN;
                existingOrganisation.EnforceMFA = Organisation.EnforceMFA;
                existingOrganisation.SubscriptionPlan = Organisation.SubscriptionPlan;
                existingOrganisation.ActiveSubscription = Organisation.ActiveSubscription;

                _organisationService.UpdateOrganisations(existingOrganisation,User.Identity.Name);
            }
            return RedirectToPage("Organisations");
        }
    }
}
