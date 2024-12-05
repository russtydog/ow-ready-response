using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    public class NotificationTemplateDetailsModel : PageModel
    {
        private readonly NotificationTemplateService _notificationTemplateService;
        private readonly UserService _userService;

        public NotificationTemplateDetailsModel(NotificationTemplateService notificationTemplateService, UserService userService)
        {
            _notificationTemplateService = notificationTemplateService;
            _userService = userService;
        }
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        [BindProperty]
        public NotificationTemplate NotificationTemplate { get; set; }

        public async Task<ActionResult> OnGetAsync(string? id)
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            if (!user.IsAdmin)
            {
                return RedirectToPage("../Dashboard");
            }
            if (id != null)
            {
                NotificationTemplate = _notificationTemplateService.GetById(Guid.Parse(id));
            }

            return Page();
        }
        public async Task<ActionResult> OnPostAsync(string? id)
        {
            var action = Request.Form["action"];

            if (string.Equals(action, "delete", StringComparison.OrdinalIgnoreCase))
            {
                //delete
                Guid guid = Guid.Parse(id);
                _notificationTemplateService.Delete(guid);
                return RedirectToPage("NotificationTemplates");
            }

            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    //create
                    var notificationCreated = _notificationTemplateService.Create(NotificationTemplate);
                    Id= notificationCreated.Id;
                }
                else
                {
                    NotificationTemplate.Id = Guid.Parse(id);
                    Id= Guid.Parse(id.ToString());
                    _notificationTemplateService.Update(Guid.Parse(id), NotificationTemplate);
                }
                return RedirectToPage("NotificationTemplates");
            }

            return Page();
        }
        public async Task<ActionResult> OnPostTemplateDesigner(string? id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    //create
                    var notificationCreated = _notificationTemplateService.Create(NotificationTemplate);
                    Id = notificationCreated.Id;
                }
                else
                {
                    NotificationTemplate.Id = Guid.Parse(id);
                    Id= Guid.Parse(id.ToString());
                    _notificationTemplateService.Update(Guid.Parse(id), NotificationTemplate);
                }
                return RedirectToPage("/Administration/NotificationTemplateDesigner", new { Id = Id });
            }
            return Page();
        }
    }
}
