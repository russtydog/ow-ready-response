using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class AssistantRequest
    {
        public string Messages { get; set; }
        public string SelectedOption { get; set; }
        public string ApiKey { get; set; }
        public string ConversationId { get; set; }
        public string LastMessage { get; set; }
        public bool IgnoreContent { get; set; }
    }
}
