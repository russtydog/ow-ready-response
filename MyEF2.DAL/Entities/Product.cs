using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime ModifiedDate { get; set; } 
        public Guid ProductTypeId { get; set; }
        public Status Status { get;set; }
        public Organisation Organisation { get; set; }
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set; }
	}
}
