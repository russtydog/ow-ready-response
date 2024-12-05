using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class NewProduct
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public Guid ProductTypeId { get; set; }
        public Guid StatusId { get; set; }  
        public Guid OrganisationId { get; set; }
        public Guid CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
	}
}
