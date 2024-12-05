using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class DeviceLoginRequest
    {
        public Guid Id { get; set; }
        public string LoginCode { get; set; }
        public string? AuthenticationCode { get; set; }
        public string? UserEmail { get; set; }
    }
}
