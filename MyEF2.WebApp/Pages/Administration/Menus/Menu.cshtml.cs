using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration.Menus
{
    [Authorize]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class MenuModel : PageModel
    {
        private readonly MenuService _menuService;
        private readonly UserService _userService;
        public MenuModel(MenuService menuService, UserService userService)
        {
            _menuService = menuService;
            _userService = userService;
        }
        [BindProperty]
        public Menu Menu { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
			User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
			if (!thisUser.IsAdmin)
            {
				return RedirectToPage("/Index");
			}
			if (id == null)
            {
				Menu = new Menu();
			}
			else
            {
				Menu = _menuService.GetMenu(Guid.Parse(id));
			}
			return Page();
		}
        public async Task<IActionResult> OnPostAsync(string? id)
        {

            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsAdmin)
            {
                return RedirectToPage("/Index");
            }

            var action = Request.Form["action"];

            if (string.Equals(action, "delete", StringComparison.OrdinalIgnoreCase))
            {
                //delete
                _menuService.DeleteMenu(Guid.Parse(id));
                return RedirectToPage("Index");
            }

            ModelState.Remove("Menu.SubMenus");
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (id == null)
            {
                var createdMenu = _menuService.AddMenu(Menu);
                return RedirectToPage("/Administration/Menus/Menu", new { id = createdMenu.Id.ToString() });
            }
            else
            {
                var existingMenu = _menuService.GetMenu(Guid.Parse(id));
                Menu.Id = Guid.Parse(id);
                Menu.MenuId = existingMenu.MenuId;
                _menuService.UpdateSubMenu(Menu);
            }
            return RedirectToPage("Index");
        }
        //create onget for handler GetSubMenu
        public async Task<IActionResult> OnGetGetSubMenuAsync(string ChildId)
        {
            var submenu = _menuService.GetMenu(Guid.Parse(ChildId));

            return new JsonResult(submenu);
        }
        //SaveSubMenu onpost handler
        public async Task<IActionResult> OnPostSaveSubMenuAsync([FromBody] SubMenu data)
        {

            //if data.Id is null, then it is a new submenu
            if (string.IsNullOrEmpty(data.Id))
            {
                
                
                //get the parent menu
                Menu menu = _menuService.GetMenu(Guid.Parse(data.ParentId));

                //create a new menu using SubMenu data
                Menu newSubMenu = new Menu
                {
                    Id = Guid.NewGuid(),
                    Name = data.Name,
                    Url = data.Url,
                    Order = data.Order,
                    Icon = data.Icon,
                    MenuId = menu.Id
                };

                newSubMenu = _menuService.AddMenu(newSubMenu);

                menu.SubMenus.Add(newSubMenu);
                _menuService.UpdateMenu(menu);
                return new JsonResult(newSubMenu);
            }
            else
            {
                //we're just updating values of the sub menu, no need to use the parent
                Menu menu = _menuService.GetMenu(Guid.Parse(data.Id));
                menu.Name = data.Name;
                menu.Url = data.Url;
                menu.Order = data.Order;
                menu.Icon = data.Icon;

                _menuService.UpdateMenu(menu);
                return new JsonResult(menu);
            }


            return new JsonResult("Success");
        }
    }
    public class  SubMenu
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public string Icon { get; set; }
        public string ParentId { get; set; }
    }
}
