using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class NotificationTemplate
    {
        public Guid Id { get; set; }
        public string TemplateName { get;set; }
        public string? TemplateHTML { get; set; }
        public string? TemplateJson { get; set; }
        public bool IsEmailNotificationTemplate { get; set; }
    }
}
