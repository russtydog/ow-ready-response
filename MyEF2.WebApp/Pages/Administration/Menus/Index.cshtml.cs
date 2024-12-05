using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration.Menus
{
    public class IndexModel : PageModel
    {
		private readonly MenuService _menuService;
		private readonly UserService _userService;
		public IndexModel(MenuService menuService, UserService userService)
		{
			_menuService = menuService;
			_userService = userService;
		}
		[BindProperty]
		public List<Menu> Menus { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
			if (!thisUser.IsAdmin)
			{
				return RedirectToPage("/Index");
			}

			Menus = _menuService.GetMenus();

			return Page();
		}
	}
}
