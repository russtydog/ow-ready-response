using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public DateTime Started { get; set; }
        public Organisation Organisation { get; set; }
        public List<Message> Messages { get; set; }
        public string? ThreadId { get; set; }
    }
}
