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
    public class OrganisationService
    {
        private readonly DatabaseContext _dbContext;
        public OrganisationService() { }
        public OrganisationService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }


        public List<Organisation> GetOrganisationList()//Admin use only
        {
            return _dbContext.Organisations.OrderBy(x=>x.OrganisationName).ToList();
        }
        public Organisation GetOrganisation(Guid id)
        {
            Organisation Organisations = _dbContext.Organisations
                .Include(x=>x.AIDocuments)
                .FirstOrDefault(x=>x.Id==id);
            return Organisations;
        }
        //When viewing information on the Organisation
        public Organisation GetOrganisation(Guid id,string UserName)
        {
            Organisation Organisations = _dbContext.Organisations
                .Include(x => x.AIDocuments)
                .FirstOrDefault(x => x.Id == id);
            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(Organisations, null, "Organisation", Organisations.Id.ToString(), Organisations, UserName, "View");
            return Organisations;
        }
        public Organisation GetOrganisationByEmail(string Email)
        {
            //EmailDomainMask on organisation will be in format of *@domain.com, see if Email matches any organisation with this mask.
            
            //Get the domain from the email
            string domain = Email.Split('@')[1];

            //Get the organisations with the matching domain mask
            Organisation organisation = _dbContext.Organisations.FirstOrDefault(x=>x.EmailDomainMask.Contains(domain));
            return organisation;

		}
        public Organisation GetOrganisationByStripeCustomerId(string StripeCustomerId)
        {
            Organisation organisation = _dbContext.Organisations.FirstOrDefault(x => x.StripeCustomerId == StripeCustomerId);
            return organisation;
        }

		public Organisation Create(Organisation organisation,string UserName)
        {
            organisation.Id = Guid.NewGuid();
            _dbContext.Add(organisation);
            _dbContext.SaveChanges();
            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(organisation, null, "Organisation", organisation.Id.ToString(), organisation, UserName, "Create");
            return organisation;
        }
        public Organisation UpdateOrganisations(Organisation organisation,string UserName)
        {
            Organisation existingOrganisation = _dbContext.Organisations.AsNoTracking().FirstOrDefault(x => x.Id == organisation.Id);


            _dbContext.Update(organisation);
            _dbContext.SaveChanges();

            AuditService auditService = new AuditService(_dbContext);
            auditService.CreateAuditFromObject(organisation, existingOrganisation, "Organisation", organisation.Id.ToString(), organisation, UserName, "Update");

            return organisation;

        }
        public Organisation UpsertFile(Guid OrganisationId, AIDocument aIDocument)
        {
            var dbOrganisation = _dbContext.Organisations
                .Include(x => x.AIDocuments)
                .FirstOrDefault(x => x.Id == OrganisationId);
            dbOrganisation.AIDocuments.Add(aIDocument);
            _dbContext.Update(dbOrganisation);
            _dbContext.SaveChanges();
            return dbOrganisation;
        }
        public AIDocument GetDocument(Guid OrganisationId, string FileName)
        {
            var dbOrganisation = _dbContext.Organisations
                .Include(x => x.AIDocuments)
                .FirstOrDefault(x => x.Id == OrganisationId);
            return dbOrganisation.AIDocuments.FirstOrDefault(x => x.DocumentName == FileName);
        }
        public AIDocument GetDocumentByFileId(Guid OrganisationId, string FileId)
        {
            var dbOrganisation = _dbContext.Organisations
                .Include(x => x.AIDocuments)
                .FirstOrDefault(x => x.Id == OrganisationId);
            return dbOrganisation.AIDocuments.FirstOrDefault(x => x.FileId == FileId);
        }
        public Organisation RemoveFile(Guid OrganisationId, AIDocument document)
        {
            var dbOrganisation = _dbContext.Organisations
                .Include(x => x.AIDocuments)
                .FirstOrDefault(x => x.Id == OrganisationId);
            dbOrganisation.AIDocuments.Remove(document);
            _dbContext.Update(dbOrganisation);
            _dbContext.SaveChanges();

            //now delete any Pictures which don't have a productId via sql statement
            _dbContext.Database.ExecuteSqlRaw("DELETE FROM AIDocuments WHERE OrganisationId IS NULL");


            return dbOrganisation;
        }
    }
}
