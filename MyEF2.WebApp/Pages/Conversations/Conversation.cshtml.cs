using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Conversations
{
    [Authorize]
    public class ConversationModel : PageModel
    {
        private readonly UserService _userService;
        private readonly ConversationService _conversationService;
        private readonly OrganisationService _organisationService;
        public ConversationModel(UserService userService, ConversationService conversationService, OrganisationService organisationService)
        {
            _userService = userService;
            _conversationService = conversationService;
            _organisationService = organisationService;
        }
        [BindProperty]
        public Conversation Conversation { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        public DateTime LocalDate(DateTime utcDate)
        {

            return new MyTime(_userService).ConvertUTCToLocalTimeForUser(true, utcDate, User.Identity.Name);
        }
        public void OnGet(string? id)
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            if (id != null)
			{
                Organisation organisation = _organisationService.GetOrganisation(user.Organisation.Id);
                Conversation = _conversationService.GetConversation(Guid.Parse(id));
            }
			if (!user.IsOrgAdmin)
			{
				Response.Redirect("/Dashboard.aspx");
			}
		}
    }
}
