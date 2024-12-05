using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class Email
    {
        public List<string> Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActionButton { get; set; }
        public string ActionButtonLabel { get; set; }
        public bool UseNotificationTemplate { get; set; }
    }
}
