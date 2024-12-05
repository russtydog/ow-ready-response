using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    public class TemplateDesignerPreviewModel : PageModel
    {
        private readonly NotificationTemplateService _notificationTemplateService;
        public TemplateDesignerPreviewModel(NotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }
        [BindProperty]
        public NotificationTemplate NotificationTemplate { get;set; }
        public void OnGet(string? id)
        {
            if (id != null)
            {
                NotificationTemplate = _notificationTemplateService.GetById(Guid.Parse(id));
            }
        }
        public async Task<ActionResult> OnPostAsync(string? id)
        {
            NotificationTemplate notificationTemplate = _notificationTemplateService.GetById(Guid.Parse(id));
            notificationTemplate.TemplateJson = NotificationTemplate.TemplateJson;
            notificationTemplate.TemplateHTML=NotificationTemplate.TemplateHTML;
            _notificationTemplateService.Update(notificationTemplate.Id,notificationTemplate);
            return RedirectToPage("NotificationTemplates");
        }
    }
}
