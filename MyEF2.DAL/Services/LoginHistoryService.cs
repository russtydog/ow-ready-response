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
    
    public class LoginHistoryService
    {
        private readonly DatabaseContext _dbContext;
        public LoginHistoryService() { }
        public LoginHistoryService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public LoginHistory AddLoginHistory(LoginHistory loginHistory)
        {
            _dbContext.LoginHistory.Add(loginHistory);
            _dbContext.SaveChanges();
            return loginHistory;
        }
        public List<LoginHistory> GetLoginHistory()
        {
            return _dbContext.LoginHistory.Include(x => x.User).Include(x => x.Organisation).ToList();
        }
        public List<LoginHistory> GetUserLoginHistory(User user)
        {
            return _dbContext.LoginHistory.Include(x => x.User).Include(x => x.Organisation).Where(x => x.User.Id == user.Id).ToList();
        }

    }
}
