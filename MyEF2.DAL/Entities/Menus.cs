using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class Menu
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public string Icon { get; set; }
        public List<Menu> SubMenus { get; set; }
        public Guid? MenuId { get; set; }
    }
}
