using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Account
{
    [Authorize]
    public class MFAStep3Model : PageModel
    {
        private readonly UserService _userService;
       
        public User user { get; set; }
        public MFAStep3Model(UserService userService)
        {
            _userService = userService;
        }
        public void OnGet()
        {
            user = _userService.GetUserByAuthId(User.Identity.Name);

        }
        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {
                user = _userService.GetUserByAuthId(User.Identity.Name);
                user.MFAEnabled = true;
                _userService.UpdateUser(user.Id, user);
                return RedirectToPage("Profile");
            }
            return Page();
        }
    }
}
