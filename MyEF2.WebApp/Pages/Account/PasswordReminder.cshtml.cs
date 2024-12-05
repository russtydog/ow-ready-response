using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Account
{
    public class PasswordReminderModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;
        private readonly UserService _userService;
        [BindProperty]
        public string EmailAddress { get; set; }
        public PasswordReminderModel(UserManager<IdentityUser> userManager,EmailService emailService, UserService userService)
        {
            this._userManager = userManager;
            this._emailService = emailService;
            _userService = userService;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(EmailAddress); // Replace with the user's username or email
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    
                    var callbackUrl = Url.Page(
                        "/Account/PasswordReset",
                        pageHandler: null,
                        values: new { userId = user.Id, code = token },
                        protocol: Request.Scheme);

                    User dbUser = _userService.GetUserByAuthUserId(Guid.Parse(user.Id));

                    Email email = new Email();
                    email.FirstName = dbUser.FirstName;
                    email.LastName = dbUser.LastName;
                    email.Subject = "Password Reset Request";
                    email.Body = "You have requested a password reset, please click the following button";
                    email.ActionButton = callbackUrl;
                    email.ActionButtonLabel = "Reset Password";
                    email.UseNotificationTemplate = true;
                    List<string> recipients = new List<string>();
                    recipients.Add(user.Email);
                    email.Recipients = recipients;
                    _emailService.SendEmail(email);

                    ModelState.AddModelError("", "A password reset request has been sent to your email address, please click the link to reset your password.");
                }
            }

            return Page();
        }
    }
}
