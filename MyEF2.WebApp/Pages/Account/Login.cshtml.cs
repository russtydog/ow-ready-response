using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;
using System.Net;
using System.Text.Encodings.Web;

namespace MyEF2.WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserService _userService;
        private readonly SettingService _settingService;
        private readonly OrganisationService _organisationService;
        private readonly LoginHistoryService _loginHistoryService;
        [BindProperty]
        public Login Model { get; set; }
        
        public LoginModel(SignInManager<IdentityUser> signInManager, UserService userService,SettingService settingService,OrganisationService organisationService,LoginHistoryService loginHistory)
        {
            this.signInManager = signInManager;
            this._userService = userService;
            this._settingService = settingService;
            this._organisationService = organisationService;
            this._loginHistoryService = loginHistory;
        }
        public async Task<IActionResult> OnGetAsync(string? returnUrl = null, string? Email=null)
        {
            //if user is logged in, go to Dashboard
            if (User.Identity.Name != null)
            {
                return RedirectToPage("../Dashboard");
            }

            Model = new Login();
            Setting settings = _settingService.GetSettings();
            if (settings.EnableSSO)
            {
				return RedirectToPage("SSO", new { ReturnURL = returnUrl });

			}
            if (Email != null)
            {
                Model.Email = Email;
            }
            if (settings.EnableOrganisationSSO&&Model.Email==null)
            {
                return RedirectToPage("LoginEmail",new { ReturnURL=returnUrl});
            }
            
            
			return Page();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            
            if (string.IsNullOrEmpty(returnUrl))
            {
                ModelState.Remove("returnUrl");
            }

            if (ModelState.IsValid)
            {
                var identityResult = await signInManager.PasswordSignInAsync(Model.Email, Model.Password, false, true);

                if (identityResult.Succeeded)
                {
                    DAL.Entities.User user = new DAL.Entities.User();
                    user = _userService.GetUserByAuthId(Model.Email);
                    var setting = _settingService.GetSettings();

                    //Log LoginHistory
                    _loginHistoryService.AddLoginHistory(new DAL.Entities.LoginHistory
                    {
                        Id = Guid.NewGuid(),
                        User = user,
                        Organisation = user.Organisation,
                        LoginTime = DateTime.UtcNow,
                    });

                    if(user.Locked)
                    {
                        //user is locked, we need to boot them
                        return RedirectToPage("AccountLocked");
                    }

                    if(setting.OrganisationsRequireSubscription && !user.Organisation.ActiveSubscription)
                    {
                        //user's organisation has no active subscription
                        return RedirectToPage("NoSubscription");
                    }

                    if (user.MFAEnabled)
                    {
                        //redirect to MFA before auth
                        //var code = user.UserId.ToString();//this is the aspnet user
                        await signInManager.SignOutAsync();

                        HttpContext.Session.SetString("UserID", user.UserId.ToString());

                        return RedirectToPage("MFAValidation", new { ReturnURL = returnUrl});


                    }
                    //User MFA isn't enabled, check if Organisation enforces it and therefore redirect to MFAStep1
                    var org = _organisationService.GetOrganisation(user.Organisation.Id);
                    if(org.EnforceMFA)
                    {
                        return RedirectToPage("MFAStep1");
                    }

                    var decodedReturnURL = WebUtility.UrlDecode("~" + returnUrl);
                    if (decodedReturnURL == "~")
                    {
                        return RedirectToPage("/Dashboard");
                    }
                    return Redirect(decodedReturnURL);
                }
                
                if(identityResult.IsNotAllowed)
                {
                    return RedirectToPage("NotAllowed");
                }
                if (identityResult.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is currently locked for at least 1 hour.");
                }
                ModelState.AddModelError("", "Username or Password incorrect");
            }

            return Page();
        }
    }
}
