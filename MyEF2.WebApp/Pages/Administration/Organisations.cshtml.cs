using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]
    public class OrganisationsModel : PageModel
    {
        private readonly UserService _userService;
        private readonly OrganisationService _organisationService;

        public OrganisationsModel(UserService userService,OrganisationService organisationService)
        {
            _userService = userService;
            _organisationService = organisationService;
        }
        public List<Organisation> Organisations { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsAdmin)
            {
                return RedirectToPage("../Index");
            }

            Organisations = _organisationService.GetOrganisationList();

            return Page();

        }
    }
}
