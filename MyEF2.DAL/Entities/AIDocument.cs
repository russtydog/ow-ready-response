using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class AIDocument
    {
        public Guid Id { get; set; }
        public Organisation Organisation { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string? FileId { get; set; }
       
    }
}
