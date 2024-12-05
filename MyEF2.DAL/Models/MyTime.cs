using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Models
{
    public class MyTime
    {
        private readonly UserService _userService;
        public MyTime() { }
        public MyTime(UserService userService)
        {
            _userService = userService;
        }

        public DateTime ConvertUTCToLocalTime(DateTime utcDateTime,string timeZone)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);

            return localTime;
        }
        public DateTime ConvertUTCToLocalTimeForUser(bool byUsername, DateTime utcDateTime,string Email)
        {
            User user = _userService.GetUserByAuthId(Email);
            if(user!=null)
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
                return localTime;
            }
            return utcDateTime;
        }
    }
}
