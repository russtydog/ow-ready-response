using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class ArticleModel
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; } 
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
    }
}
