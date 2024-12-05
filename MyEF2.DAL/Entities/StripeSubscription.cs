using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class StripeSubscription
    {
        public Guid Id { get; set; }

        public Organisation Organisation { get; set; }
        public string? StripeSessionId { get; set; }
        public string? StripePlanId { get; set; }
        public string? StripeSubscriptionId { get; set; }
        public string? StripeProductPriceId { get; set; }
    }
}
