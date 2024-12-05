using Microsoft.AspNetCore.Identity;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using System.Transactions;

namespace MyEF2.WebApp
{
    public class MyEF2TokenProvider : IUserTwoFactorTokenProvider<IdentityUser>
    {
        private readonly UserService _userService;
        public MyEF2TokenProvider(UserService userService)
        {
            _userService = userService;
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<IdentityUser> manager, IdentityUser user)
        {
            return Task.FromResult(true);

        }

        public async Task<string> GenerateAsync(string purpose, UserManager<IdentityUser> manager, IdentityUser user)
        {
            int tokenLength = 12;
            string alphanumericToken = GenerateRandomAlphanumericToken(tokenLength);

            string tokenWithPurpose = $"{purpose}:{alphanumericToken}";

            return tokenWithPurpose;

        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<IdentityUser> manager, IdentityUser user)
        {
            string[] tokenParts = token.Split(':');

            if (tokenParts.Length != 2)
            {
                return false;
            }

            string tokenPurpose = tokenParts[0];
            string tokenValue = tokenParts[1];

            if (tokenPurpose != purpose)
            {
                return false;
            }

            User dbUser = _userService.GetUserByAuthUserId(Guid.Parse(user.Id));

            if (token == dbUser.OTPCode)  //use the full token as this is what is stored in the User table
            {
                return true;
            }
            

            return false;
        }
        private string GenerateRandomAlphanumericToken(int length)
        {
            
            const string alphanumericCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string token = new string(Enumerable.Repeat(alphanumericCharacters, length)
                                                .Select(s => s[random.Next(s.Length)])
                                                .ToArray());

            return token;
        }
    }
}
