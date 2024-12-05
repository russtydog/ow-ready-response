using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Services;
using TwoFactorAuthNet;
using static TwoFactorAuthNet.TwoFactorAuth;
using Microsoft.AspNetCore.Authorization;


namespace MyEF2.WebApp.Pages.Account
{
    [Authorize]
    public class MFAStep2Model : PageModel
    {
        private readonly UserService _userService;
        [BindProperty]
        public string OTPCode { get; set; }
        [BindProperty]
        public string MFASecret { get; set; }
        public MFAStep2Model(UserService userService)
        {
            _userService = userService;
        }
        public void OnGet()
        {
            DAL.Entities.User user = _userService.GetUserByAuthId(User.Identity.Name);
            MFASecret = user.MFASecret;
        }
        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {
                string userSecret = MFASecret; // Load this from your database
                string userCode = OTPCode; // The 6-digit code they enter

                var tfa = new TwoFactorAuth("MyEF2");

                bool isValid = tfa.VerifyCode(MFASecret, OTPCode);

                if (isValid)
                {
                    // The code is valid, MFA is successful
                    // You can mark the user as MFA-verified or proceed with the secure action
                    return RedirectToPage("MFAStep3");
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
