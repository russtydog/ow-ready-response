using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using MyEF2.DAL.DatabaseContexts;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserService userService;
        private readonly SettingService settingService;
        private readonly EmailService _emailService;
        private readonly OrganisationService organisationService;

        [BindProperty]
        public Registration Registration { get; set; }
        [BindProperty]
        public User NewUser { get; set; }

        public RegisterModel(UserManager<IdentityUser> userManager,SignInManager<IdentityUser> signInManager, UserService userService, SettingService settingService, EmailService emailService, OrganisationService organisationService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.userService = userService;
            this.settingService = settingService;
            this._emailService = emailService;
            this.organisationService = organisationService;
        }
        public async Task<IActionResult> OnGetAsync(string? userId, string? code,string? Organisation)
        {
            Setting s = settingService.GetSettings();
            if(!s.EnableRegistration)
            {
                return RedirectToPage("Login");
            }
            if(!string.IsNullOrEmpty(userId)&&!string.IsNullOrEmpty(code))
            {
                var user = await userManager.FindByIdAsync(userId);

                if(user!=null)
                {
                    var result = await userManager.ConfirmEmailAsync(user, code);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                        return RedirectToPage("../Dashboard");
                    }
                }
            }
            if(!string.IsNullOrEmpty(Organisation))
            {
                
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string? Organisation)
        {
            ModelState.Remove("NewUser.Email");
            ModelState.Remove("NewUser.Profile");
            ModelState.Remove("NewUser.IsAdmin");
            ModelState.Remove("NewUser.MFAEnabled");
            ModelState.Remove("NewUser.MFASecret");
            ModelState.Remove("NewUser.DarkMode");
            ModelState.Remove("NewUser.OTPCode");
            ModelState.Remove("NewUser.Organisation");
            ModelState.Remove("NewUser.IsOrgAdmin");
            ModelState.Remove("NewUser.TimeZone");
            ModelState.Remove("NewUser.APIKey");
            if (ModelState.IsValid)
            {
                if(!Registration.AgreeTermsOfService)
                {
                    ModelState.AddModelError("", "Must agree to terms of service");
                    return Page();
                }
                var user = new IdentityUser()
                {
                    
                    UserName = Registration.Email,
                    Email = Registration.Email

                };

                var result = userManager.CreateAsync(user, Registration.Password);
                if(result.Result.Succeeded)
                {
                    var checkExistingUserCount = userService.GetUsers().Count;

                    User newUser = new User();
                    newUser.Id = Guid.NewGuid();
                    newUser.FirstName = NewUser.FirstName;
                    newUser.LastName = NewUser.LastName;
                    newUser.Email = Registration.Email;
                    newUser.UserId = new Guid(user.Id);
                    newUser.Profile = "";
                    newUser.IsAdmin=checkExistingUserCount==0?true:false;
                    newUser.MFAEnabled=false;
                    newUser.DarkMode = false;
                    newUser.OTPCode = "";
                    newUser.TimeZone = "AUS Eastern Standard Time";
                    newUser.DisplayTheme = 2;

                    if (!string.IsNullOrEmpty(Organisation))
                    {
                        //invited from an organiation
                        var organisation = organisationService.GetOrganisation(Guid.Parse(Organisation));
                        if(organisation!=null)
                        {
                            newUser.Organisation=organisation;  
                            newUser.IsOrgAdmin = false;
                        }
                    }

                    if(newUser.Organisation==null)
                    {
                        if (string.IsNullOrEmpty(settingService.GetSettings().DefaultOrganisationId))
                        {
                            var newOrganisation = new Organisation();
                            newOrganisation.OrganisationName = "My Organisation";
                            newOrganisation.ABN = "";
                            organisationService.Create(newOrganisation,newUser.Email);
                            newUser.Organisation = newOrganisation;
                            newUser.IsOrgAdmin = true;
                        }
                        else
                        {
                            Organisation org = new Organisation();
                            org = organisationService.GetOrganisation(Guid.Parse(settingService.GetSettings().DefaultOrganisationId));
                            newUser.Organisation = org;
                            newUser.IsOrgAdmin = false;
                        }
                    }

                    string userTOTPSecret = TOTPGenerator.GenerateTOTPSecret();
                    newUser.MFASecret=userTOTPSecret;
                    string userAPIKey = TOTPGenerator.GenerateTOTPSecret();
                    newUser.APIKey = userAPIKey;
                    newUser.Locked=false;

                    userService.CreateUser(newUser);

                    //await signInManager.SignInAsync(user,false);
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    

                    // Check if the current request's query string contains "StripeProductID"
                    var stripeProductId = HttpContext.Request.Query["StripeProductId"].FirstOrDefault();

                    // Create an anonymous object for the values parameter, always including StripeProductID
                    // If StripeProductID is not present, its value will be null
                    var values = new { 
                        userId = user.Id, 
                        code = token, 
                        StripeProductID = stripeProductId // This will be null if stripeProductId is null
                    };

                    var confirmationLink = Url.Page(
                        "/Account/EmailVerification", pageHandler: null,
                        values: values, Request.Scheme);

                    // Send the confirmation link to the user's email address
                    // You can use a service like SendGrid or your own email provider.
                    Email email = new Email();
                    email.Subject = "Email Verification";
                    email.Body="Please click the button below to verify your Email Address.";
                    email.ActionButton = confirmationLink;
                    email.ActionButtonLabel = "Verify Now";
                    email.UseNotificationTemplate = true;

                    email.Recipients = new List<string>();
                    email.Recipients.Add(Registration.Email);
                    email.FirstName = newUser.FirstName;
                    email.LastName = newUser.LastName;
                    
                    _emailService.SendEmail(email);

                    return RedirectToPage("RegisterConfirmation");
                }

                foreach (var item in result.Result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

            }
            return Page();
        }
        

    }
}
