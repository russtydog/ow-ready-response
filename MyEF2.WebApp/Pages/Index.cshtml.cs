using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages
{
    
    public class IndexModel : PageModel
    {
        
        private readonly SettingService _settingService;

        public IndexModel(SettingService settingService)
        {
            _settingService = settingService;
        }
        public Setting Setting { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Setting = _settingService.GetSettings();
            if(Setting.UseFrontend==false)
            {
                return RedirectToPage("/Dashboard");
            }
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Dashboard");
            }

            return Page();
        }
    }
}