using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration.Audits
{
    [Authorize]
	[IgnoreAntiforgeryToken(Order = 1001)]

	public class IndexModel : PageModel
    {
        private readonly UserService _userService;
        private readonly AuditService _auditService;

        public IndexModel(UserService userService, AuditService auditService)
        {
            _userService = userService;
            _auditService = auditService;
        }
		public DateTime LocalDate(DateTime utcDate)
		{

			return new MyTime(_userService).ConvertUTCToLocalTimeForUser(true, utcDate, User.Identity.Name);
		}
		public List<DAL.Entities.Audit> Audits { get; set; }
        public void OnGet()
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            if(!user.IsAdmin && !user.IsOrgAdmin)
            {
                RedirectToPage("/Dashboard");
            }

            if(user.IsAdmin)
            {
				Audits = _auditService.GetAll().OrderByDescending(a => a.Date).ToList();
				return;
			}
            Audits = _auditService.GetAll(user.Organisation).OrderByDescending(a => a.Date).ToList();
        }
        public async Task<IActionResult> OnPostAuditsAsync()
        {
			var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
			var start = HttpContext.Request.Form["start"].FirstOrDefault();
			var length = HttpContext.Request.Form["length"].FirstOrDefault();
			var searchValue = HttpContext.Request.Form["search[value]"].FirstOrDefault();

			int pageSize = length != null ? Convert.ToInt32(length) : 0;
			int skip = start != null ? Convert.ToInt32(start) : 0;
			int recordsTotal = 0;

            List<Audit> records;
			User user = _userService.GetUserByAuthId(User.Identity.Name);
            if (user.IsAdmin)
            {
                records = _auditService.GetAll().OrderByDescending(x=>x.Date).ToList();
            }
            else { 
                records = _auditService.GetAll(user.Organisation).OrderByDescending(x=>x.Date).ToList();
            }

            List<Audit> filteredRecords = null;
			if (!(string.IsNullOrEmpty(searchValue)) && records != null)
			{
				//records = records.AsQueryable().SearchAllFields(searchValue).ToList();
				filteredRecords = records.Where(record =>
					record.PropertyName.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
	                record.EntityName.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
	                record.UserName.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
	                record.Action.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
	                record.EntityId.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0 ||
	                (record.NewValue!=null && record.NewValue.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0) ||
	                (record.OldValue!=null && record.OldValue.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
					).ToList();

			}
            else
            {
                filteredRecords = records;
            }

			recordsTotal = filteredRecords.Count();

            var myTime = new MyTime();

            var data = filteredRecords.Skip(skip).Take(pageSize).Select(record => new
            {
                Id = record.Id,
                Date = new MyTime().ConvertUTCToLocalTime(record.Date,user.TimeZone).ToString("yyyy-MM-dd HH:mm:ss"),
                UserName = record.UserName,
                EntityName = record.EntityName,
                PropertyName = record.PropertyName,
                OldValue = record.OldValue,
                NewValue = record.NewValue,
                Action = record.Action,
                EntityId = record.EntityId,
                Url="<a href='/Administration/Audits/Audit/"+record.Id+ "'><i class=\"fas fa-edit\"></i></a>"
			});  

            //in data there is a date field, need to update the date field by passing it into the MyTime function to convert to local datetime




            return new JsonResult(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
		}
	}
}
