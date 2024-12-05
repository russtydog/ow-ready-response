using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class NotificationTemplateService
    {
        private readonly DatabaseContext _dbContext;
        public NotificationTemplateService() { }
        public NotificationTemplateService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public NotificationTemplate Create(NotificationTemplate notificationTemplates)
        {
            notificationTemplates.Id = Guid.NewGuid();
            _dbContext.Add(notificationTemplates);
            _dbContext.SaveChanges();
            return notificationTemplates;
        }
        public NotificationTemplate Update(Guid id, NotificationTemplate notificationTemplate)
        {
            
            
            _dbContext.Update(notificationTemplate);
            _dbContext.SaveChanges();
            return notificationTemplate;
        }
        public List<NotificationTemplate> GetAll()
        {
            return _dbContext.NotificationTemplates.ToList();
        }
        public NotificationTemplate GetById(Guid id)
        {
            return _dbContext.NotificationTemplates.FirstOrDefault(x => x.Id == id);
        }
        public string Delete(Guid id)
        {
            NotificationTemplate notificationTemplate = _dbContext.NotificationTemplates.FirstOrDefault(x => x.Id == id);
            _dbContext.NotificationTemplates.Remove(notificationTemplate);
            _dbContext.SaveChanges();
            return "Record Deleted";
        }
    }
}
