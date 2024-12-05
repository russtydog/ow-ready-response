using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class ConversationMessageStarterResponse
    {
        public string ConversationId { get; set; }
        public string ThreadId { get; set; }
        public string MessageId { get; set; }
        public string AssistantId { get; set; }
        public int RequestTokens { get; set; }
    }
}
