using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages
{
    public class TermsModel : PageModel
    {
        private readonly SettingService _settingService;
        public TermsModel(SettingService settingService)
        {
            _settingService = settingService;
        }
        [BindProperty]
        public string? TermsOfService { get; set; }
        public void OnGet()
        {
            TermsOfService = _settingService.GetSettings().TermsOfService;

            //TermsOfService is markdown, convert it to html using Markdig
            TermsOfService = Markdig.Markdown.ToHtml(TermsOfService);

        }
    }
}
