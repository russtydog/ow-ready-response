using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using MyEF2.DAL.Models;

namespace MyEF2.WebApp.Pages.Account
{
   
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly UserService _userService;
        private readonly SettingService _settingService;
        private readonly LoginHistoryService _loginHistoryService;

        [BindProperty]
        public User UserModel { get; set; }
        [BindProperty]
        public IFormFile UploadFile { get; set; }

        public List<TimeZoneInfo> TimeZones { get; set; }

        public string APIUrl { get; set; }
        public ProfileModel(UserService userService,SettingService settingService,LoginHistoryService loginHistoryService)
        {
            _userService = userService;
            _settingService = settingService;
            _loginHistoryService = loginHistoryService;
        }
        public DateTime LastLogin { get; set; }
        public void OnGet()
        {
            TimeZones = TimeZoneInfo.GetSystemTimeZones().ToList();
            
            UserModel = _userService.GetUserByAuthId(User.Identity.Name);

            APIUrl = _settingService.GetSettings().APIUrl;

            //it's just display so convert Created and Modified dates to local time
            UserModel.DateCreated = new MyTime().ConvertUTCToLocalTime(UserModel.DateCreated, UserModel.TimeZone);
            UserModel.DateModified = new MyTime().ConvertUTCToLocalTime(UserModel.DateModified, UserModel.TimeZone);

            //if User has encrypted their API key, decrypt it
            if (UserModel.APIKey != null && UserModel.APIKey != "" && UserModel.APIKey.EndsWith("="))
            {
                UserModel.APIKey = Encryption.Decrypt(UserModel.APIKey);
            }

            //order the list of GetUserLoginHistory descending and get the first login date
            List<LoginHistory> loginHistory = _loginHistoryService.GetUserLoginHistory(UserModel);
            
            LastLogin = loginHistory.OrderByDescending(x => x.LoginTime).First().LoginTime;
            LastLogin = new MyTime().ConvertUTCToLocalTime(LastLogin, UserModel.TimeZone);



            if (String.IsNullOrEmpty(UserModel.Profile)||UserModel.Profile=="")
            {
                UserModel.Profile = "../dist/img/UserProfile.png";
            }
            
            if(String.IsNullOrEmpty(UserModel.MFASecret)||UserModel.MFASecret=="")
            {
                string userTOTPSecret = TOTPGenerator.GenerateTOTPSecret();
                UserModel.MFASecret = userTOTPSecret;
            }
        }
        public IActionResult OnPostSaveChanges()
        {
            
            if (UploadFile != null && UploadFile.Length > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(UploadFile.FileName);
                string filePath = Path.Combine("wwwroot/uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
                UserModel.Profile = fileName;
            }


            ModelState.Remove("UploadFile");
            ModelState.Remove("UserModel.OTPCode");
            ModelState.Remove("UserModel.Organisation");

            if (ModelState.IsValid)
            {
                DAL.Entities.User userUpdates = new User();
                userUpdates = _userService.GetUserByAuthId(User.Identity.Name);
                userUpdates.FirstName = UserModel.FirstName;
                userUpdates.LastName = UserModel.LastName;
                userUpdates.Email = UserModel.Email;
                userUpdates.Profile=UserModel.Profile;
                userUpdates.IsAdmin = UserModel.IsAdmin;
                userUpdates.MFAEnabled = UserModel.MFAEnabled;
                userUpdates.MFASecret=UserModel.MFASecret;
                userUpdates.DarkMode = UserModel.DarkMode;
                userUpdates.IsOrgAdmin = UserModel.IsOrgAdmin;
                userUpdates.TimeZone=UserModel.TimeZone;
                userUpdates.APIKey = Encryption.Encrypt(UserModel.APIKey);
                userUpdates.DisplayTheme = UserModel.DisplayTheme;

                _userService.UpdateUser(userUpdates.Id, userUpdates);

                return RedirectToPage("Profile");
            }
            return Page();
        }
        public IActionResult OnPostGenerateNewKey()
        {
            string userAPIKey = "sk_" + TOTPGenerator.GenerateTOTPSecret();
            UserModel.APIKey = userAPIKey;
            return OnPostSaveChanges();
        }
    }
}
