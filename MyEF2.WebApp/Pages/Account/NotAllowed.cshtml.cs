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
using MyEF2.DAL.Models;

namespace MyEF2.WebApp.Pages.Account
{
   
    public class NotAllowedModel : PageModel
    {

        private readonly UserService _userService;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly EmailService emailService;
        [BindProperty]
        public string EmailAddress { get; set; }
        
        public NotAllowedModel(SignInManager<IdentityUser> signInManager,UserService userService, UserManager<IdentityUser> userManager,EmailService emailService)
        {
            this.signInManager = signInManager;
            _userService = userService;
            this.userManager=userManager;
            this.emailService=emailService;
        }
        public void OnGet()
        {
            
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(EmailAddress); // Replace with the user's username or email
                if (user != null)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    
                    var callbackUrl = Url.Page(
                        "/Account/EmailVerification",
                        pageHandler: null,
                        values: new { userId = user.Id, code = token },
                        protocol: Request.Scheme);

                    User dbUser = _userService.GetUserByAuthUserId(Guid.Parse(user.Id));

                    Email email = new Email();
                    email.FirstName = dbUser.FirstName;
                    email.LastName = dbUser.LastName;
                    email.Subject = "Email Verification";
                    email.Body = "You have requested a to re-verify your email address, please click this button";
                    email.ActionButton = callbackUrl;
                    email.ActionButtonLabel = "Verify Now";
                    email.UseNotificationTemplate = true;
                    List<string> recipients = new List<string>();
                    recipients.Add(user.Email);
                    email.Recipients = recipients;
                    emailService.SendEmail(email);

                    ModelState.AddModelError("", "An email verification has been sent to your email address, please click the link to be able to login.");
                }
            }

            return Page();
        }
        
    }
}
