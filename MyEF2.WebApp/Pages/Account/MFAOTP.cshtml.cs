using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;
using System.Security.Claims;

namespace MyEF2.WebApp.Pages.Account
{
    public class MFAOTPModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserService _userService;
        private readonly EmailService _emailService;

        [BindProperty]
        public string EmailAddress { get; set; }

        public MFAOTPModel(SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager, EmailService emailService,UserService userService)
        {
            this._userManager = userManager;
            _emailService = emailService;
            _userService = userService;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> OnGetAsync(string? userId, string? code)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(code))
            {
                var user = await _userManager.FindByIdAsync(userId);
                string purpose = "MFAOTP";
                
                var validate = await _userManager.VerifyUserTokenAsync(user, "AppTokenProvider", purpose, code);
                if(validate==true)
                {
                    //now run custom auth
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

                    //given we have used the code, clear it
                    User dbUser = _userService.GetUserByAuthUserId(Guid.Parse(user.Id));
                    dbUser.OTPCode = "";
                    _userService.UpdateUser(dbUser.Id, dbUser);
                    return RedirectToPage("../Dashboard");
                }
                await _signInManager.SignOutAsync();

                ModelState.AddModelError("", "An error occurred validating your one time password via email. Please try again");
            }
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(EmailAddress); // Replace with the user's username or email
                if (user != null)
                {
                    //var token = await _userManager.GenerateUserTokenAsync(user, "MyEF2", "MFAReset");
                    string purpose = "MFAOTP";

                    var token = await _userManager.GenerateUserTokenAsync(user, "AppTokenProvider", purpose);

                  DAL.Entities.User dbUser = _userService.GetUserByAuthUserId(Guid.Parse(user.Id));

                    var callbackUrl = Url.Page(
                        "/Account/MFAOTP",
                        pageHandler: null,
                        values: new { userId = user.Id, code = token },
                        protocol: Request.Scheme);

                    Email email = new Email();
                    email.Subject = "MFA One Time Password";
                    email.Body = "You have requested a one time password via email. Please click the below button";
                    email.ActionButton = callbackUrl;
                    email.ActionButtonLabel = "Verify Now";
                    email.UseNotificationTemplate = true;

                    List<string> recipients = new List<string>();
                    recipients.Add(user.Email);
                    email.Recipients = recipients;
                    email.FirstName = dbUser.FirstName;
                    email.LastName = dbUser.LastName;
                    
                    _emailService.SendEmail(email);

                    //store token in User table so it can only be used once.
                    User dbuser = _userService.GetUserByAuthUserId(Guid.Parse(user.Id));
                    dbuser.OTPCode= token;
                    _userService.UpdateUser(dbuser.Id, dbuser);


                    ModelState.AddModelError("", "A one time password has been sent to your email address, please click the link to login. Once you have logged in, ensure you reset your Authenticator app");
                }
            }
            return Page();
        }
    }
}
