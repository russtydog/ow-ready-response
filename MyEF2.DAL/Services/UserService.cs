using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class UserService
    {
        private readonly DatabaseContext _dbContext;
        public UserService() { }
        public UserService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<User> GetUsers()
        {

            return _dbContext.Users.ToList().OrderBy(p => p.LastName).ToList();
        }
        public List<User> GetUsersForOrganisation(string OrganisationID)
        {
            return _dbContext.Users.Where(u => u.Organisation.Id == Guid.Parse(OrganisationID)).ToList().OrderBy(u=>u.LastName).ToList();     
        }

        public User GetUser(Guid Id)
        {
            User user = _dbContext.Users.Include(x=>x.Organisation).FirstOrDefault(x => x.Id == Id);
            return user;
        }
        public User GetUserByAuthId(string username)
        {
            User user = _dbContext.Users.Include(x => x.Organisation).FirstOrDefault(x => x.Email == username);
            return user;
        }
        public User GetUserByAuthUserId(Guid userid)
        {
            User user = _dbContext.Users.Include(x => x.Organisation).FirstOrDefault(x => x.UserId == userid);
            return user;
        }
        public User GetUserByAPIKey(string apiKey)
        {
            //API keys are now encrypted. Need to encrypt the provided key to ee if it matches a user
            apiKey = Encryption.Encrypt(apiKey);
            User user = _dbContext.Users.Include(x => x.Organisation).FirstOrDefault(x => x.APIKey == apiKey);
            return user;
        }
        public User UpdateUser(Guid id, User user)
        {
            User dbUser = _dbContext.Users.FirstOrDefault(x => x.Id == id);

            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Email = user.Email;
            dbUser.DateModified= DateTime.UtcNow;
            dbUser.Profile = user.Profile;
            dbUser.IsAdmin=user.IsAdmin;
            dbUser.MFAEnabled=user.MFAEnabled;
            dbUser.MFASecret = user.MFASecret;
            dbUser.DarkMode=user.DarkMode;
            dbUser.OTPCode = user.OTPCode;
            dbUser.IsOrgAdmin=user.IsOrgAdmin;
            dbUser.TimeZone = user.TimeZone;
            dbUser.APIKey = user.APIKey;
            dbUser.Locked=user.Locked;
            dbUser.DisplayTheme = user.DisplayTheme;
            // End Stadard Fields and Properties



            _dbContext.Update(dbUser);
            _dbContext.SaveChanges();

            return dbUser;

        }
        public User CreateUser(User user)
        {

            User newUser = new User();
            newUser.Id = Guid.NewGuid();
            newUser.FirstName = user.FirstName;
            newUser.LastName = user.LastName;
            newUser.Email = user.Email;
            newUser.UserId = user.UserId;
            newUser.DateCreated=DateTime.UtcNow;
            newUser.DateModified = DateTime.UtcNow;
            newUser.Profile = "";
            newUser.IsAdmin = user.IsAdmin;
            newUser.MFAEnabled = user.MFAEnabled;
            newUser.MFASecret = user.MFASecret;
            newUser.DarkMode = user.DarkMode;
            newUser.OTPCode=user.OTPCode;
            newUser.Organisation = user.Organisation;
            newUser.IsOrgAdmin=user.IsOrgAdmin;
            newUser.TimeZone = user.TimeZone;
            newUser.APIKey = user.APIKey;
            newUser.Locked=user.Locked;
            newUser.DisplayTheme= user.DisplayTheme;
            _dbContext.Add(newUser);
            _dbContext.SaveChanges();
            return newUser;
            
        }
        public string DeleteUser(Guid id)
        {
            try
            {
                User user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
                return "Successfully Deleted User";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public DateTime ConvertUTCDateToUserTimezone(DateTime utcDate, Guid UserId)
        {
            User user = _dbContext.Users.FirstOrDefault(x => x.Id == UserId);
            //user.TimeZone is the TimeZone ID

            return TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone));
        }
    }
}
