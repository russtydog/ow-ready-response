using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    public class NotificationTemplateDesignerModel : PageModel
    {
        private readonly NotificationTemplateService _notificationTemplateService;
        private readonly UserService _userService;

        public NotificationTemplateDesignerModel(NotificationTemplateService notificationTemplateService, UserService userService)
        {
            _notificationTemplateService = notificationTemplateService;
            _userService = userService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }
        [BindProperty]
        public NotificationTemplate NotificationTemplate { get; set; }
        public string jSonURL { get; set; }
        public async Task<ActionResult> OnGetAsync(string? id)
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            if (!user.IsAdmin)
            {
                return RedirectToPage("../Dashboard");
            }
            if (id != null)
            {
                Id = id;

                NotificationTemplate = _notificationTemplateService.GetById(Guid.Parse(id));
                var retval = "https://rsrc.getbee.io/api/templates/m-bee";
                if (!string.IsNullOrEmpty(NotificationTemplate.TemplateJson))
                {
                    var baseUrl = new Uri(HttpContext.Request.GetEncodedUrl()).GetLeftPart(UriPartial.Authority);

                    retval = baseUrl + "/Administration/NotificationTemplateJSon/"+id; //this needs to be a url
                }
                jSonURL = retval;
            }
            return Page();
        }
    }
}
