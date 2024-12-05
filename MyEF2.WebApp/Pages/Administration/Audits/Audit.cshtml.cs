using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration.Audits
{
    [Authorize]
    public class AuditModel : PageModel
    {
        private readonly AuditService _auditService;
        private readonly UserService _userService;
        public AuditModel(AuditService auditService, UserService userService)
        {
            _auditService = auditService;
            _userService = userService;
        }
        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }
        public Audit Audit { get; set; }
        public void OnGet(string? id)
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            if (!user.IsAdmin && !user.IsOrgAdmin)
            {
                RedirectToPage("/Dashboard");
            }
            if (id != null)
            {
                //edit mode
                Audit = _auditService.GetAudit(Guid.Parse(id));
				Audit.Date = new MyTime(_userService).ConvertUTCToLocalTimeForUser(true, Audit.Date, User.Identity.Name);

			}
		}
    }
}
