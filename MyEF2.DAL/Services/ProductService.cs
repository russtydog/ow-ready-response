using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Migrations;
using MyEF2.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class ProductService
    {
        private readonly DatabaseContext _dbContext;
        public ProductService() { }
        public ProductService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Product> GetProducts(Guid organisationId)
        {
           
            return _dbContext.Products.Include(m=>m.ModifiedBy).Include(c=>c.CreatedBy).Include(o=>o.Organisation).Include(p=>p.Status).ToList().OrderBy(p => p.Price).Where(x=>x.Organisation.Id==organisationId).ToList();
        }

        public Product GetProduct(Guid ProductID,string? UserName)
        {
            Product product = _dbContext.Products.Include(m => m.ModifiedBy).Include(c => c.CreatedBy).Include(o => o.Organisation).Include(p => p.Status).FirstOrDefault(x => x.Id == ProductID);
            
            if(UserName!=null)
            {
                AuditService auditService = new AuditService(_dbContext);
                auditService.CreateAuditFromObject(product, null, "Product", product.Id.ToString(), product.Organisation, UserName,"View");
            }
            
            return product;
        }
        public Product UpdateProduct(Guid ProductID,NewProduct newProduct)
        {
            Product originalProduct = _dbContext.Products.AsNoTracking().FirstOrDefault(x => x.Id == ProductID);
            Product DBProduct = _dbContext.Products.FirstOrDefault(x => x.Id == ProductID);

            DBProduct.ProductName = newProduct.ProductName;
            DBProduct.Price = newProduct.Price;
            DBProduct.ModifiedDate = DateTime.UtcNow;
            DBProduct.ProductTypeId= newProduct.ProductTypeId;
            
            var status = _dbContext.Statuses.FirstOrDefault(x => x.Id == newProduct.StatusId);
            var modifiedby = _dbContext.Users.FirstOrDefault(x => x.Id == newProduct.ModifiedById);

            DBProduct.Status = status;
            DBProduct.ModifiedBy = modifiedby;

            _dbContext.Update(DBProduct);
            _dbContext.SaveChanges();

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(DBProduct, originalProduct, "Product", DBProduct.Id.ToString(), DBProduct.Organisation, modifiedby.Email,"Update");


            return DBProduct;

        }
        public Product CreateProduct(NewProduct newProduct)
        {

            Product product = new Product();
            product.Id=Guid.NewGuid();
            product.CreatedDate=DateTime.UtcNow;
            product.ModifiedDate=DateTime.UtcNow;

            product.ProductName = newProduct.ProductName;
            product.Price = newProduct.Price;
            product.ProductTypeId = newProduct.ProductTypeId;

            var status = _dbContext.Statuses.FirstOrDefault(x => x.Id == newProduct.StatusId);
            var organisation = _dbContext.Organisations.FirstOrDefault(x => x.Id == newProduct.OrganisationId);
            var createdby = _dbContext.Users.FirstOrDefault(c=>c.Id == newProduct.CreatedById);
            var modifiedby = _dbContext.Users.FirstOrDefault(c=>c.Id == newProduct.ModifiedById);

			product.Organisation = organisation;
            product.CreatedBy = createdby;
            product.ModifiedBy = modifiedby;

			if (status != null)
            {
                product.Status= status;
                _dbContext.Add(product);
                _dbContext.SaveChanges();

                //create audit for each property of product
                AuditService auditService = new AuditService(_dbContext);
                auditService.CreateAuditFromObject(product,null, "Product", product.Id.ToString(), organisation, createdby.Email,"Create");

                return product;
            }
            else
            {
                return null;
            }

            
            return product;
        }
        public string DeleteProduct(Guid ProductID,string? UserName)
        {
            try
            {
                Product product = _dbContext.Products.Include(x=>x.Organisation).FirstOrDefault(x => x.Id == ProductID);
                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();
                AuditService auditService = new AuditService(_dbContext);
                auditService.CreateAuditFromObject(product, null, "Product", product.Id.ToString(), product.Organisation, UserName, "Delete");
                return "Successfully Deleted Product";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
