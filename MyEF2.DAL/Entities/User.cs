using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }   
        public Guid UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Profile { get; set; }
        public bool IsAdmin { get; set; }
        public bool MFAEnabled { get; set; }
        public string MFASecret { get; set; }
        public bool DarkMode { get; set; }
        public string OTPCode { get; set; }
        public Organisation Organisation { get; set; }
        public bool IsOrgAdmin { get; set; }
        public string TimeZone { get; set; }
        public string APIKey { get; set; }
        public bool Locked{get;set;}
        public int DisplayTheme { get; set; }
        // end standard fieldset 
    }
}
