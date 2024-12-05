using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class ProductTypeService
    {
        private readonly DatabaseContext _dbContext;
        public ProductTypeService() { }
        public ProductTypeService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<ProductTypes> GetProductTypes()
        {

            return _dbContext.ProductTypes.ToList().OrderBy(p => p.ProductType).ToList();
        }
        public ProductTypes CreateProductType(ProductTypes productType)
        {
            productType.Id = Guid.NewGuid();
            _dbContext.ProductTypes.Add(productType);
            _dbContext.SaveChanges();
            return productType;
        }
    }
}
