using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class Audit
    {
        public Guid Id { get; set; }
        public Organisation? Organisation { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? PropertyName { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime Date { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
    }
}
