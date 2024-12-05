using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Account
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserService _userService;

        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }
        [BindProperty]
        public string CurrentPassword { get; set; }


        public ChangePasswordModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,UserService userService)
        {
            this._userManager = userManager;
            this.signInManager = signInManager; 
            this._userService = userService;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                if(Password==ConfirmPassword)
                {
                    var identityResult = await signInManager.PasswordSignInAsync(User.Identity.Name, CurrentPassword, false, false);
                    if (identityResult.Succeeded)
                    {
                        var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                        if (user != null)
                        {
                            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                            var result = await _userManager.ResetPasswordAsync(user, resetToken, Password);

                            if (result.Succeeded)
                            {
                                User userModel = _userService.GetUserByAuthId(User.Identity.Name);
                                userModel.DateModified=DateTime.UtcNow;
                                _userService.UpdateUser(userModel.Id, userModel);
                                // Password successfully updated
                                return RedirectToPage("Profile");
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Current Password failed authentication. " + identityResult.ToString());
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Password and Password Confirmation don't match");
                }
            }
            return Page();
        }
    }
}
