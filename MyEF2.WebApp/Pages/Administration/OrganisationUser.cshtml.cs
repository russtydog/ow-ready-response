using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]


    public class OrganisationUserModel : PageModel
    {
        private readonly UserService _userService;
        private readonly SettingService _settingService;
        private readonly EmailService _emailService;
        private readonly UserManager<IdentityUser> _userManager;
        
        [BindProperty]
        public bool isEdit{get;set;}
        [BindProperty]
        public User UserModel {get;set;}
        public OrganisationUserModel(UserService userService,UserManager<IdentityUser> userManager,EmailService emailService,SettingService settingService)
        {
            _userService=userService;
            _userManager=userManager;
            _emailService=emailService;
            _settingService=settingService;
        }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if(id==null)
            {
                User thisUserCheck = _userService.GetUserByAuthId(User.Identity.Name);
                Setting setting = _settingService.GetSettings();
                if (!thisUserCheck.IsOrgAdmin||!setting.UseOrganisations)
                {
                    return RedirectToPage("../Dashboard");
                }
                isEdit=false;
            }
            else
            {
                User thisUserCheck = _userService.GetUserByAuthId(User.Identity.Name);
                Setting setting = _settingService.GetSettings();
                if (!thisUserCheck.IsOrgAdmin||!setting.UseOrganisations)
                {
                    return RedirectToPage("../Dashboard");
                }
                isEdit=true;
            }

            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsOrgAdmin)
            {
                return RedirectToPage("../Dashboard");
            }
            if(id!=null)
            {
                UserModel = _userService.GetUser(Guid.Parse(id));
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string? id)
        {
            ModelState.Remove("UserModel.Profile");
            ModelState.Remove("UserModel.MFASecret");
            ModelState.Remove("UserModel.OTPCode");
            ModelState.Remove("UserModel.Organisation");
            ModelState.Remove("UserModel.TimeZone");
            ModelState.Remove("UserModel.APIKey");

            if (ModelState.IsValid)
            {
                if(id==null)
                {
                    //create user
                    var identityUser = new IdentityUser()
                    {
                        
                        UserName = UserModel.Email,
                        Email = UserModel.Email

                    };
                    var result = _userManager.CreateAsync(identityUser);
                    if(result.Result.Succeeded)
                    {
                        DAL.Entities.User adminUser = _userService.GetUserByAuthId(User.Identity.Name);
                        UserModel.Organisation=adminUser.Organisation;
                        UserModel.UserId=new Guid(identityUser.Id);
                        UserModel.Profile="";
                        UserModel.OTPCode="";
                        UserModel.TimeZone = "AUS Eastern Standard Time";
                        string userTOTPSecret = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.MFASecret=userTOTPSecret;
                        string userAPIKey = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.APIKey = userAPIKey;
                        UserModel.DisplayTheme = 2;

                        _userService.CreateUser(UserModel);

                        //Send email
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                        var settings = _settingService.GetSettings();

                        var confirmationLink = Url.Page(
                        "/Account/EmailVerification", pageHandler: null,
                            values: new { userId = identityUser.Id, code = token }, Request.Scheme);

                        // Send the confirmation link to the user's email address
                        // You can use a service like SendGrid or your own email provider.
                        Email email = new Email();
                        email.Subject = "Email Verification";
                        email.Body="You have been invited to " + settings.ApplicationName + ". Please click the button below to verify your account.";
                        email.ActionButton = confirmationLink;
                        email.ActionButtonLabel = "Verify Now";
                        email.UseNotificationTemplate = true;

                        email.Recipients = new List<string>();
                        email.Recipients.Add(UserModel.Email);
                        email.FirstName=UserModel.FirstName;
                        email.LastName=UserModel.LastName;
                        
                        
                        _emailService.SendEmail(email);
                        return RedirectToPage("Organisation");
                    }
                    foreach (var item in result.Result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
                else
                {
                    //update user
                    User dbUser = _userService.GetUser(Guid.Parse(id));
                    dbUser.FirstName=UserModel.FirstName;
                    dbUser.LastName=UserModel.LastName;
                    dbUser.Email=UserModel.Email;
                    dbUser.Locked=UserModel.Locked;
                    dbUser.IsOrgAdmin=UserModel.IsOrgAdmin;
                    _userService.UpdateUser(Guid.Parse(id),dbUser);
                    return RedirectToPage("Organisation");

                }
            }
           
            return Page();
        }
    }
}
