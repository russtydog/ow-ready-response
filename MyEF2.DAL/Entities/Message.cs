using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Conversation Conversation { get; set; }
        public string Response { get; set; }
        public string Sender { get; set; }
        public DateTime Created { get; set; }
        public int Tokens { get; set; }
        public string? StreamStatus { get; set; }
        public string? MessageStream { get; set; }

    }
}
