using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class Article
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public Organisation Organisation { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime Modified { get; set; }
    }
}
