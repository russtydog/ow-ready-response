using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Saml;

namespace MyEF2.WebApp.Pages.Account
{
    
    public class LoginEmailModel : PageModel
    {
        private readonly UserService _userService;
        private readonly OrganisationService _organisationService;
        private readonly SettingService _settingService;
        private readonly LoginHistoryService _loginHistoryService;

        public LoginEmailModel(UserService userService, OrganisationService organisationService,SettingService settingService,LoginHistoryService loginHistoryService)
        {
            _userService = userService;
            _organisationService = organisationService;
            _settingService=settingService;
            _loginHistoryService=loginHistoryService;
        }
        [BindProperty]
        public string Email { get; set; }

        public void OnGet()
        {
            if (User.Identity.Name != null)
            {
                Response.Redirect("../Index");
            }

            Setting setting = _settingService.GetSettings();
            if (!setting.EnableOrganisationSSO)
            {
                
                Response.Redirect("Login");
            }
        }
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            //user has posted their email address, get their user so the organisation can be retrieved, then redirect to /SAML/{orgId}
            User user = _userService.GetUserByAuthId(Email);
            if(user==null)
            {
                //user doesn't exist. Lets use the email to see if there's a matching email domain mask on any organisation
                Organisation orgMatch = _organisationService.GetOrganisationByEmail(Email);
                if(orgMatch!=null)
                {
					//check if sso is enabled for this organisation
					if(orgMatch.EnableSSO)
                    {
						//return to the login page and pass the Email address to query string
						var samlEndpointAuto = orgMatch.LoginURL;

						string strReturnURLAuto = returnUrl != null ? "?ReturnUrl=" + returnUrl : "";
						string urlAuto = Request.Scheme + "://" + Request.Host;
						string redirAuto = urlAuto + "/SAML/" + orgMatch.Id.ToString();

						var requestAuto = new AuthRequest(
							orgMatch.EntityID,
							redirAuto + strReturnURLAuto
							);

						return Redirect(requestAuto.GetRedirectUrl(samlEndpointAuto));
					}
				}
                return Page();
            }
            Organisation organisation = _organisationService.GetOrganisation(user.Organisation.Id);
            if(!organisation.EnableSSO)
            {
                //return to the login page and pass the Email address to query string
                return RedirectToPage("Login", new { Email = Email, ReturnURL = returnUrl });
                
            }


            var samlEndpoint = organisation.LoginURL;

            string strReturnURL = returnUrl != null ? "?ReturnUrl=" + returnUrl : "";
            string url = Request.Scheme + "://" + Request.Host;
            string redir = url + "/SAML/" + organisation.Id.ToString();

            //record the login history now to capture the return URL
            LoginHistory loginHistory = new LoginHistory
            {
                User = user,
                Organisation = organisation,
                LoginTime = DateTime.Now,
                ReturnUrl = returnUrl
            };
            _loginHistoryService.AddLoginHistory(loginHistory);

            var request = new AuthRequest(
                organisation.EntityID,
                redir + strReturnURL
                );

            return Redirect(request.GetRedirectUrl(samlEndpoint));
        }
        
    }
}
