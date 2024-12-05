using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class AuditService
    {
        private readonly DatabaseContext _dbContext;
        public AuditService()
        {
        }
        public AuditService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
		public List<Audit> GetAll()
		{
			return _dbContext.Audits.ToList();
		}
		public List<Audit> GetAll(Organisation organisation)
        {
            return _dbContext.Audits.Where(x=>x.Organisation==organisation).ToList();
        }
        public Audit GetAudit(Guid id)
        {
            return _dbContext.Audits.FirstOrDefault(x => x.Id == id);
        }
        public void Add(Audit audit)
        {
            _dbContext.Audits.Add(audit);
            _dbContext.SaveChanges();
        }
        public Audit Update(Audit audit)
        {
            _dbContext.Audits.Update(audit);
            _dbContext.SaveChanges();
            return audit;
        }

        public void CreateAuditFromObject(object obj,object? originalobject, string EntityName, string EntityId, Organisation? organisation,string UserName,string Action)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                object propertyValue = property.GetValue(obj, null);

                Audit audit = new Audit();

                //if originalobject is not null, then get the original value of the property
                if (originalobject != null && Action == "Update")
                {

                    object originalValue = property.GetValue(originalobject, null);
                    if (!Equals(originalValue, propertyValue))
                    {
                        audit.Organisation = organisation;
                        audit.Date = DateTime.UtcNow;
                        audit.EntityId = EntityId;
                        audit.EntityName = EntityName;
                        audit.NewValue = propertyValue==null ?"": propertyValue.ToString();
                        audit.OldValue = originalValue==null?"":originalValue.ToString();
                        audit.PropertyName = propertyName;
                        audit.UserName = UserName;
                        audit.Action = Action;
                        Add(audit);
                    }
                }
                else if (Action == "Create")
                {
                    audit.Organisation = organisation;
                    audit.Date = DateTime.UtcNow;
                    audit.EntityId = EntityId;
                    audit.EntityName = EntityName;
                    audit.NewValue = propertyValue==null?"": propertyValue.ToString();
                    audit.PropertyName = propertyName;
                    audit.UserName = UserName;
                    audit.Action = Action;
                    Add(audit);
                }
                else if (Action == "Delete")
                {
                    audit.Organisation = organisation;
                    audit.Date = DateTime.UtcNow;
                    audit.EntityId = EntityId;
                    audit.EntityName = EntityName;
                    audit.OldValue = propertyValue==null ? "" : propertyValue.ToString();
                    audit.PropertyName = propertyName;
                    audit.UserName = UserName;
                    audit.Action = Action;
                    Add(audit);
                }
                else if (Action == "View")
                {
                    audit.Organisation = organisation;
                    audit.Date = DateTime.UtcNow;
                    audit.EntityId = EntityId;
                    audit.EntityName = EntityName;
                    audit.NewValue = propertyValue==null?"": propertyValue.ToString();
                    audit.OldValue = null;
                    audit.PropertyName = propertyName;
                    audit.UserName = UserName;
                    audit.Action = Action;
                    Add(audit);
                }
            }
        }

    }
}
