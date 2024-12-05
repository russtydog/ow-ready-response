using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Conversations
{
    
    [Authorize]

    public class IndexModel : PageModel
    {
        
        private readonly ConversationService _conversationService;
        private readonly UserService _userService;
        private readonly OrganisationService _organisationService;

        public IndexModel(ConversationService conversationService, UserService userService, OrganisationService organisationService)
        {
            _conversationService = conversationService;
            _userService = userService;
            _organisationService = organisationService;
        }
        [BindProperty]
        public List<Conversation> Conversations { get; set; }
		public DateTime LocalDate(DateTime utcDate)
		{

			return new MyTime(_userService).ConvertUTCToLocalTimeForUser(true, utcDate, User.Identity.Name);
		}
		public void OnGet()
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            Organisation organisation = _organisationService.GetOrganisation(user.Organisation.Id);
            Conversations = _conversationService.GetAll(organisation);

            if (!user.IsOrgAdmin)
            {
                Response.Redirect("/Dashboard.aspx");
            }
        }
    }
}
