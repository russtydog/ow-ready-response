using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class DeviceLoginRequestService
    {
        private readonly DatabaseContext _dbContext;
        public DeviceLoginRequestService() { }
        public DeviceLoginRequestService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public DeviceLoginRequest AddDeviceLoginRequest(DeviceLoginRequest deviceLoginRequest)
        {
            _dbContext.DeviceLoginRequests.Add(deviceLoginRequest);
            _dbContext.SaveChanges();
            return deviceLoginRequest;
        }
        public DeviceLoginRequest GetDeviceLoginRequest(Guid Id)
        {
            return _dbContext
                .DeviceLoginRequests
                .FirstOrDefault(x => x.Id == Id);
        }
        public DeviceLoginRequest GetDeviceLoginRequest(string LoginCode)
        {
            return _dbContext
                .DeviceLoginRequests
                .FirstOrDefault(x => x.LoginCode == LoginCode);
        }
        public DeviceLoginRequest GetDeviceLoginRequest(bool byAuthenticationCode,string AuthenticationCode)
        {
            return _dbContext
                .DeviceLoginRequests
                .FirstOrDefault(x => x.AuthenticationCode == AuthenticationCode);
        }
        public DeviceLoginRequest UpdateDeviceLoginRequest(DeviceLoginRequest deviceLoginRequest)
        {
            _dbContext.DeviceLoginRequests.Update(deviceLoginRequest);
            _dbContext.SaveChanges();
            return deviceLoginRequest;
        }
        public string DeleteDeviceLoginRequest(Guid Id)
        {
            var deviceLoginRequest = _dbContext.DeviceLoginRequests.FirstOrDefault(x => x.Id == Id);
            if (deviceLoginRequest != null)
            {
                _dbContext.DeviceLoginRequests.Remove(deviceLoginRequest);
                _dbContext.SaveChanges();
                return "Device Login Request Deleted Successfully";
            }
            return "Device Login Request Not Found";
        }
    }
}
