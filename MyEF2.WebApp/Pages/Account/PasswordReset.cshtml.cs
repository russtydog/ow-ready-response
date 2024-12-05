using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;

namespace MyEF2.WebApp.Pages.Account
{
    
public class PasswordResetModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public PasswordResetModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public PasswordConfirmModel PasswordConfirm { get; set; }

        public async Task<IActionResult> OnGetAsync(string? userId, string? code)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(code))
            {
                var user = await _userManager.FindByIdAsync(userId);

            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string? userId, string? code)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, code, PasswordConfirm.Password);
                    if (result.Succeeded)
                    {
                        // Password has been reset successfully
                        // You can redirect the user to a success page or login page
                        return RedirectToPage("Login");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return Page();
        }
    }
    

}
