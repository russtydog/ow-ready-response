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
   
    public class AccountLockedModel : PageModel
    {

        private readonly UserService _userService;
        private readonly SignInManager<IdentityUser> signInManager;
        
        public AccountLockedModel(SignInManager<IdentityUser> signInManager,UserService userService)
        {
            this.signInManager = signInManager;
            _userService = userService;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            await signInManager.SignOutAsync();
            return Page();
        }
        
    }
}
