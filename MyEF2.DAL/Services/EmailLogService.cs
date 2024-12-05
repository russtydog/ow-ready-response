using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class EmailLogService
    {
        private readonly DatabaseContext _dbContext;
        public EmailLogService() { }
        public EmailLogService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public EmailLog Create(EmailLog emailLog)
        {
            _dbContext.Add(emailLog);
            _dbContext.SaveChanges();
            return emailLog;
        }
    }
}
