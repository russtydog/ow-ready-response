using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class StripeSubscriptionService
    {
        private readonly DatabaseContext _dbContext;
        public StripeSubscriptionService() { }
        public StripeSubscriptionService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public StripeSubscription GetStripeSubscription(Guid StripeSubscriptionID)
        {
            return _dbContext.StripeSubscriptions
                .Include(x => x.Organisation)
                .FirstOrDefault(x => x.Id == StripeSubscriptionID);
        }
        public StripeSubscription Create(StripeSubscription stripeSubscription)
        {
            stripeSubscription.Id = Guid.NewGuid();
            _dbContext.StripeSubscriptions.Add(stripeSubscription);
            _dbContext.SaveChanges();
            return stripeSubscription;
        }
        public StripeSubscription Update(StripeSubscription stripeSubscription)
        {
            _dbContext.StripeSubscriptions.Update(stripeSubscription);
            _dbContext.SaveChanges();

            return stripeSubscription;
        }
        public void Delete(StripeSubscription stripeSubscription)
        {
            _dbContext.StripeSubscriptions.Remove(stripeSubscription);
            _dbContext.SaveChanges();
        }
    }
}
