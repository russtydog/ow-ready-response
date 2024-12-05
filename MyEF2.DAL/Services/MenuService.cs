using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class MenuService
    {
        private readonly DatabaseContext _dbContext;
        public MenuService() { }
        public MenuService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<Menu> GetMenus()
        {
            return _dbContext
                .Menus
                .Include(x=> x.SubMenus)
                .OrderBy(x=> x.Order)
                .ToList();
        }
        public Menu GetMenu(Guid Id)
        {
            return _dbContext
                .Menus
                .Include(x => x.SubMenus)
                .FirstOrDefault(x => x.Id == Id);
        }
        public Menu AddMenu(Menu menu)
        {
            _dbContext.Menus.Add(menu);
            _dbContext.SaveChanges();
            return menu;
        }
        public Menu UpdateMenu(Menu menu)
        {
            _dbContext.Menus.Update(menu);
            _dbContext.SaveChanges();
            return menu;
        }
        public Menu UpdateSubMenu(Menu submenu)
        {
            var existingSubMenu = _dbContext.Menus.FirstOrDefault(x => x.Id == submenu.Id);
            if (existingSubMenu != null)
            {
                existingSubMenu.Name = submenu.Name;
                existingSubMenu.Url = submenu.Url;
                existingSubMenu.Order = submenu.Order;
                existingSubMenu.Icon = submenu.Icon;
                _dbContext.Menus.Update(existingSubMenu);
                _dbContext.SaveChanges();
            }
            return existingSubMenu;
        }
        public void DeleteMenu(Guid Id)
        {
            var menu = _dbContext.Menus.Include(x=>x.SubMenus).FirstOrDefault(x=>x.Id==Id);
            if (menu != null)
            {
                //if deleting a parent menu, delete all submenus
                if (menu.SubMenus != null)
                {
                    foreach (var submenu in menu.SubMenus)
                    {
                        _dbContext.Menus.Remove(submenu);
                    }
                }
                _dbContext.Menus.Remove(menu);
                _dbContext.SaveChanges();
            }
        }
    }
}
