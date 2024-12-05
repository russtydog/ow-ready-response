using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class ConversationCreateResponse
    {
        public string FinalResponse { get; set; }
        public int RequestTokens { get; set; }
        public int ResponseTokens { get; set; }
    }
}
