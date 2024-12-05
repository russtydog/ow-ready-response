using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Account
{
   
    public class EmailVerificationModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        
        
        private readonly EmailService _emailService;
        public EmailVerificationModel(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            
        }
        public async Task<IActionResult> OnGetAsync(string? userId, string? code,string? Organisation,string? StripeProductId)
        {
            if(!string.IsNullOrEmpty(userId)&&!string.IsNullOrEmpty(code))
            {
                var user = await _userManager.FindByIdAsync(userId);

                if(user!=null)
                {
                    var result = await _userManager.ConfirmEmailAsync(user, code);
                    if (result.Succeeded)
                    {
                        if(!string.IsNullOrEmpty(StripeProductId))
                        {
                            //Sign in the user
                            await _signInManager.SignInAsync(user, false);
                            return RedirectToPage("/Pricing", new { StripeProductId = StripeProductId });
                        }

                        return RedirectToPage("../Dashboard");
                    }
                }
            }
            if(!string.IsNullOrEmpty(Organisation))
            {
                
            }
            return Page();
        }
    }

    
}