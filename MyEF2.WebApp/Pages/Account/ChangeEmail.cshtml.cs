using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Account
{
    [Authorize]
    public class ChangeEmailModel : PageModel
    {
        private readonly UserService _userService;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string ConfirmEmail { get; set; }
        public ChangeEmailModel(UserService userService, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (Email != ConfirmEmail)
            {
                ModelState.AddModelError("", "Email and Confirm Email do not match. Please Correct");

            }
            else
            {
                var OldEmail = User.Identity.Name;
                var identityResult = await signInManager.PasswordSignInAsync(OldEmail, Password, false, true);

                if (identityResult.Succeeded)
                {
                    
                    var existingUser = await userManager.FindByEmailAsync(Email);

                    if (existingUser != null && existingUser.Email != User.Identity.Name)
                    {
                        
                        ModelState.AddModelError("", "This email is already registered to another user");

                    }
                    else
                    {
                        var user = await userManager.FindByEmailAsync(User.Identity.Name);
                        user.Email = Email;
                        user.UserName = Email;
                        var result = await userManager.UpdateAsync(user);
                        var UserModel = _userService.GetUserByAuthUserId(Guid.Parse(user.Id));
                        UserModel.Email = Email;
                        UserModel.DateModified = DateTime.UtcNow;
                        _userService.UpdateUser(UserModel.Id, UserModel);

                        if(result.Succeeded)
                        {
                            //logout and redirect to login
                            await signInManager.SignOutAsync();
                            return RedirectToPage("../Dashboard");

                        }
                    }
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
