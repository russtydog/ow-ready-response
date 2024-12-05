using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class StatusService
    {
        private readonly DatabaseContext _dbContext;
        public StatusService() { }
        public StatusService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Status> GetStatuses()
        {
            return _dbContext.Statuses.ToList().OrderBy(p => p.StatusName).ToList();

        }
        public Status GetStatus(Guid StatusID)
        {
            Status status = _dbContext.Statuses.FirstOrDefault(x => x.Id == StatusID);
            return status;
        }
        public Status CreateStatus(NewStatus newStatus)
        {
            Status status = new Status();
            status.Id = Guid.NewGuid();
            status.StatusName = newStatus.StatusName;
            _dbContext.Statuses.Add(status);
            _dbContext.SaveChanges();
            return status;
        }
        public string DeleteStatus(Guid StatusID)
        {
            try
            {
                Status status = GetStatus(StatusID);
                if (status != null)
                {

                    _dbContext.Statuses.Remove(status);
                    _dbContext.SaveChanges();
                    return "Successfully Deleted Status";
                }
                else
                {
                    return "Status Not Found";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
