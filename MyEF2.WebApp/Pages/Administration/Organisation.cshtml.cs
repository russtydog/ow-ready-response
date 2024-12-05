using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using System.ComponentModel;

namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]

    public class OrganisationModel : PageModel
    {
        private readonly UserService _userService;
        private readonly SettingService _settingService;
        private readonly OrganisationService _organisationService;
        private readonly StripeProductService _stripeProductService;
        public OrganisationModel(UserService userService, OrganisationService organisationService,SettingService settingService,StripeProductService stripeProductService)
        {
            _userService = userService;
            _organisationService = organisationService;
            _settingService=settingService;
            _stripeProductService=stripeProductService;
        }
        [BindProperty]
        public Organisation Organisation { get; set; }
        public List<User> Users { get; set; }
        public bool OrganisationSSOEnabled { get; set; }
        public string ReplyUrl { get; set; }    
        public bool EnableStripeForOrgs { get; set; }
        public string SelectedPlan { get;set; }
        public async Task<IActionResult> OnGetAsync()
        {
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            Setting setting = _settingService.GetSettings();
            if (!thisUser.IsOrgAdmin||!setting.UseOrganisations)
            {
                return RedirectToPage("../Dashboard");
            }
            
            Organisation = _organisationService.GetOrganisation(thisUser.Organisation.Id,thisUser.Email);
            Users = _userService.GetUsersForOrganisation(thisUser.Organisation.Id.ToString());
            OrganisationSSOEnabled = setting.EnableOrganisationSSO;
            EnableStripeForOrgs = setting.OrganisationsRequireSubscription;
            
            if(Organisation.SubscriptionPlan!=null)
            {
                
                var stripeProduct=_stripeProductService.GetStripeProduct(Guid.Parse(Organisation.SubscriptionPlan), thisUser.Email);   
                SelectedPlan= stripeProduct.Name + " ($" + stripeProduct.Amount.ToString("N2") + "/" + stripeProduct.Frequency + ")";
            }

            //get the current website url excluding the page
            string url = Request.Scheme + "://" + Request.Host;
            //build the URL so it's in a format of https://localhost:44300/SAML/12345678-1234-1234-1234-123456789012
            
            ReplyUrl = url + "/SAML/" + Organisation.Id.ToString();


            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Organisation.AIDocuments");
            if(ModelState.IsValid)
            {

                var organisation = _organisationService.GetOrganisation(Organisation.Id);
                organisation.OrganisationName=Organisation.OrganisationName;
                organisation.ABN= Organisation.ABN;
                organisation.EnforceMFA= Organisation.EnforceMFA;
                organisation.EnableSSO=Organisation.EnableSSO;
                organisation.EnableAutoSSORegistration=Organisation.EnableAutoSSORegistration;
                organisation.SkipEmailVerification=Organisation.SkipEmailVerification;
                organisation.EmailDomainMask=Organisation.EmailDomainMask;
                organisation.EntityID=Organisation.EntityID;
                organisation.LoginURL=Organisation.LoginURL;
                organisation.SigningCertificate=Organisation.SigningCertificate;
                organisation.AskAIAPI=Organisation.AskAIAPI;
                organisation.AskAIAPIKey=Organisation.AskAIAPIKey;
                organisation.AskAIAssistantId=Organisation.AskAIAssistantId;

                _organisationService.UpdateOrganisations(organisation,User.Identity.Name);
                return RedirectToPage("../Dashboard");
            }
            return Page();
        }
    }
}
