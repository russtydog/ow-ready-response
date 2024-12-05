using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class StripeProduct
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? StatementDescriptor { get; set; }
        public string? Frequency { get; set; }
        public string? Image { get; set; }
        public string? StripeProductId { get; set; }
        public Organisation Organisation { get; set; }
		public decimal Amount { get; set; }
		public string? Currency { get; set; }
		public string? PlanId { get; set; }
        public string? PaymentLink { get; set; }
        public int? TrialPeriodDays { get; set; }
        public string? Features{get;set;}
        public bool HideFromPricing { get; set; }
        // End Stadard Fields and Properties

	}
}
