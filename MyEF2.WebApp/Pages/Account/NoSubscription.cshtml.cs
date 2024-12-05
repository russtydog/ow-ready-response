using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyEF2.WebApp.Pages.Account
{
   
    public class NoSubscription : PageModel
    {
        private readonly UserService _userService;
        private readonly SettingService _settingService;
        [BindProperty]
        public User UserModel { get; set; }
        [BindProperty]
        public string StripeCustomerPortalUrl { get; set; }
        public NoSubscription(UserService userService, SettingService settingService)
        {
            _userService = userService;
            _settingService = settingService;
        }
        
        public void OnGet()
        {
            UserModel = _userService.GetUserByAuthId(User.Identity.Name);
            Setting setting = _settingService.GetSettings();
            StripeCustomerPortalUrl = setting.StripeCustomerPortalUrl;
        }

    }
}