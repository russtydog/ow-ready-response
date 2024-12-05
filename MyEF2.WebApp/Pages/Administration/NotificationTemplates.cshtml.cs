using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    public class NotificationTemplatesModel : PageModel
    {
        private readonly UserService _userService;
        private readonly NotificationTemplateService _notificationTemplateService;

        public NotificationTemplatesModel(UserService userService, NotificationTemplateService notificationTemplateService)
        {
            _userService = userService;
            _notificationTemplateService = notificationTemplateService;
        }

        public List<DAL.Entities.NotificationTemplate> NotificationTemplates { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);

            if(!user.IsAdmin)
            {
                return RedirectToPage("../Dashboard");
            }

            NotificationTemplates = _notificationTemplateService.GetAll();

            return Page();
        }
    }
}
