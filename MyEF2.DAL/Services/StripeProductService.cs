using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class StripeProductService
    {
        private readonly DatabaseContext _dbContext;
        public StripeProductService() { }
        public StripeProductService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<StripeProduct> GetStripeProducts()
        {
            return _dbContext.StripeProducts
				.ToList()
				//.Where(x => x.Organisation == organisation)
                .OrderBy(p => p.Name).ToList();

        }
        public StripeProduct GetStripeProduct(Guid id,string UserName)
        {
            return _dbContext.StripeProducts.FirstOrDefault(p => p.Id == id);
                
        }
        public StripeProduct GetStripeProduct(string PlanId)
        {
            return _dbContext.StripeProducts.FirstOrDefault(p => p.PlanId == PlanId);

        }
        
        public StripeProduct Create(StripeProduct stripeProduct,string UserName)
        {
            stripeProduct.Id = Guid.NewGuid();
            _dbContext.StripeProducts.Add(stripeProduct);
            _dbContext.SaveChanges();
            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(stripeProduct, null, "StripeProduct", stripeProduct.Id.ToString(), stripeProduct.Organisation, UserName, "Create");
            return stripeProduct;
        }
        public StripeProduct Update(StripeProduct stripeProduct, string UserName)
        {
            // Fetch the existing product from the database
            var originalProduct = _dbContext.StripeProducts
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == stripeProduct.Id);
            var existingProduct = _dbContext.StripeProducts
                .FirstOrDefault(p => p.Id == stripeProduct.Id);

            if (existingProduct != null)
            {
                // Update the existing product's properties
                existingProduct.Name = stripeProduct.Name;
                existingProduct.Description = stripeProduct.Description;
                existingProduct.Frequency = stripeProduct.Frequency;
                existingProduct.Id = stripeProduct.Id;
                existingProduct.Image = stripeProduct.Image;
                existingProduct.StatementDescriptor = stripeProduct.StatementDescriptor;
                existingProduct.StripeProductId = stripeProduct.StripeProductId;
                existingProduct.Amount = stripeProduct.Amount;
                existingProduct.Currency = stripeProduct.Currency;
                existingProduct.PlanId = stripeProduct.PlanId;
                existingProduct.PaymentLink = stripeProduct.PaymentLink;
                existingProduct.TrialPeriodDays=stripeProduct.TrialPeriodDays;
                existingProduct.Features = stripeProduct.Features;
                existingProduct.HideFromPricing=stripeProduct.HideFromPricing;

                // Save changes to the database
                _dbContext.SaveChanges();
            }
            else
            {
                // If the product doesn't exist, create it
                _dbContext.StripeProducts.Add(stripeProduct);
                _dbContext.SaveChanges();
            }

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(stripeProduct, originalProduct, "StripeProduct", stripeProduct.Id.ToString(), stripeProduct.Organisation, UserName, "Update");

            return stripeProduct;
        }
        
        public string Delete(Guid StripeProductID,string UserName)
        {
            try
            {
                StripeProduct stripeProduct = GetStripeProduct(StripeProductID,UserName);
                if (stripeProduct != null)
                {

                    _dbContext.StripeProducts.Remove(stripeProduct);
                    _dbContext.SaveChanges();
                    AuditService auditService = new AuditService(_dbContext);
                    auditService.CreateAuditFromObject(stripeProduct, null, "StripeProduct", stripeProduct.Id.ToString(), stripeProduct.Organisation, UserName, "Delete");
                    return "Successfully Deleted StripeProduct";
                }
                else
                {
                    return "StripeProduct Not Found";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
