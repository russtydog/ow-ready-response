using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class LoginHistory
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Organisation Organisation { get; set; }
        public DateTime LoginTime { get; set; }
        public string? ReturnUrl{get;set;}
    }
}
