using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using TwoFactorAuthNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Net;

namespace MyEF2.WebApp.Pages.Account
{
   
    public class MFAValidationModel : PageModel
    {

        private readonly UserService _userService;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        [BindProperty]
        public string MFACode { get; set; }
        public string UserID { get; set; }
        public MFAValidationModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,UserService userService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _userService = userService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var code = HttpContext.Session.GetString("UserID");

            if (!string.IsNullOrEmpty(code) )
            {
                UserID= code;
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string? returnUrl = null, string? id=null)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                ModelState.Remove("returnUrl");
            }
            var code = HttpContext.Session.GetString("UserID");
            UserID = code;

            if (ModelState.IsValid)
            {
                DAL.Entities.User user = _userService.GetUserByAuthUserId(Guid.Parse(UserID));
                string userSecret = user.MFASecret; // Load this from your database
                string userCode = MFACode; // The 6-digit code they enter

                var tfa = new TwoFactorAuth("MyEF2");

                bool isValid = tfa.VerifyCode(userSecret, userCode);

                if (isValid)
                {
                    // The code is valid, MFA is successful
                    // You can mark the user as MFA-verified or proceed with the secure action
                    //if (returnUrl == null || returnUrl == "/")
                    //{
                        //signInManager.ExternalLoginSignInAsync("AppTokenProvider", user.UserId.ToString(), false);

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Email), // Add the username claim
                            new Claim(ClaimTypes.Email,user.Email)
                        };
                        var identity = new ClaimsIdentity(claims, "ApplicationCookie");
                        var principal = new ClaimsPrincipal(identity);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = false,
                            ExpiresUtc = DateTime.UtcNow.AddHours(1)
                        };
                        await HttpContext.SignInAsync("Identity.Application", principal, authProperties);


                        // Redirect to the decoded URL
                        var decodedReturnURL = WebUtility.UrlDecode("~" + returnUrl);
                       if (decodedReturnURL == "~")
                       {
                           return RedirectToPage("/Index");
                       }
                       return Redirect(decodedReturnURL);
                    //}
                    //else
                    //{
                    //    return RedirectToPage(returnUrl);
                    //}
                }
                else
                {
                    // The code is not valid, MFA failed
                    // You can handle this as needed, such as displaying an error message
                    ModelState.AddModelError("", "Entered Code does not match your account secret or has expired");
                }
            }
            return Page();
        }
    }
}
