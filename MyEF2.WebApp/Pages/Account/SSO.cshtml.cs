using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Saml;

namespace MyEF2.WebApp.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class SSOModel : PageModel
    {
        private readonly SettingService _settingService;

        public SSOModel(SettingService settingService)
        {
            _settingService = settingService;
        }
        
        public IActionResult OnGet(string? returnUrl = null)
        {
            DAL.Entities.Setting s = _settingService.GetSettings();

            var samlEndpoint = s.LoginURL;

            string strReturnURL = returnUrl != null ? "?ReturnUrl=" + returnUrl : "";

            string redir = Url.Page(
                    "/Account/saml", pageHandler: null,
                        values: null, Request.Scheme);

            var request = new AuthRequest(
                s.EntityID, 
                redir + strReturnURL 
                );

            return Redirect(request.GetRedirectUrl(samlEndpoint));
        }
    }
}
