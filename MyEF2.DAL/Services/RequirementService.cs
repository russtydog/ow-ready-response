using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;

namespace MyEF2.DAL.Services
{
    public class RequirementService
    {
        private readonly DatabaseContext _dbContext;
        public RequirementService() { }
        public RequirementService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Requirement> List()
        {
            return _dbContext.Requirements.ToList();
        }

        public Requirement Get(int id)
        {
            return _dbContext.Requirements.Find(id);
        }

        public Requirement Add(Requirement requirement)
        {
            _dbContext.Requirements.Add(requirement);
            _dbContext.SaveChanges();
            return requirement;
        }

        public Requirement Update(Requirement requirement)
        {
            _dbContext.Entry(requirement).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return requirement;
        }

        public void Delete(int id)
        {
            var requirement = _dbContext.Requirements.Find(id);
            if (requirement != null)
            {
            _dbContext.Requirements.Remove(requirement);
            _dbContext.SaveChanges();
            }
        }

        
    }
}