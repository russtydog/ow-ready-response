using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    public class NotificationTemplateJsonModel : PageModel
    {
        private readonly NotificationTemplateService _notificationTemplateService;

        public NotificationTemplateJsonModel(NotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        public async Task<ActionResult> OnGetAsync(string? id)
        {
            if(id!=null)
            {
                NotificationTemplate notificationTemplate = _notificationTemplateService.GetById(Guid.Parse(id));
                return new JsonResult(notificationTemplate.TemplateJson);
            }
            return Page();
        }
    }
}
