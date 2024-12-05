using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class EmailLog
    {
        public Guid Id { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }    
        public DateTime Created { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
    }
}
