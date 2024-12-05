using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;


namespace MyEF2.WebApp.Pages.Administration
{
    [Authorize]
    public class GlobalAdminOrganisationUserModel : PageModel
    {
        private readonly UserService _userService;
        private readonly OrganisationService _organisationService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;

        public GlobalAdminOrganisationUserModel(UserService userService, OrganisationService organisationService, UserManager<IdentityUser> userManager, EmailService emailService)
        {
            _userService = userService;
            _organisationService = organisationService;
            _userManager = userManager;
            _emailService = emailService;
        }
        public string OrganisationIDParam { get; set; }
        public string UserIDParam { get; set; }
        [BindProperty]
        public DAL.Entities.User UserModel { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            if (HttpContext.Request.Query["OrganisationID"] != "")
            {
                OrganisationIDParam = HttpContext.Request.Query["OrganisationID"];
            }
            if (HttpContext.Request.Query["UserID"] != "")
            {
                UserIDParam = HttpContext.Request.Query["UserID"];
            }

            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsAdmin)
            {
                return RedirectToPage("../Index");
            }

            

            //if UserIDParam="Create" then we are creating a new user, if it is a guid then we need to get existing user
            if (UserIDParam == "Create")
            {
                return Page();
            }
            else
            {
                UserModel = _userService.GetUser(Guid.Parse(UserIDParam));
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Request.Query["OrganisationID"] != "")
            {
                OrganisationIDParam = HttpContext.Request.Query["OrganisationID"];
            }
            if (HttpContext.Request.Query["UserID"] != "")
            {
                UserIDParam = HttpContext.Request.Query["UserID"];
            }

            Organisation organisation = _organisationService.GetOrganisation(Guid.Parse(OrganisationIDParam));

            ModelState.Remove("UserModel.Profile");
            ModelState.Remove("UserModel.MFASecret");
            ModelState.Remove("UserModel.OTPCode");
            ModelState.Remove("UserModel.Organisation");
            ModelState.Remove("UserModel.TimeZone");
            ModelState.Remove("UserModel.APIKey");
            if (ModelState.IsValid)
            {
                if (UserIDParam == "Create")
                {
                    //create user
                    var identityUser = new IdentityUser()
                    {

                        UserName = UserModel.Email,
                        Email = UserModel.Email

                    };
                    var result = _userManager.CreateAsync(identityUser);
                    if (result.Result.Succeeded)
                    {
                        //create a new user
                        UserModel.Organisation = organisation;
                        UserModel.UserId = new Guid(identityUser.Id);
                        UserModel.Profile = "";
                        UserModel.OTPCode = "";
                        UserModel.TimeZone = "AUS Eastern Standard Time";
                        string userTOTPSecret = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.MFASecret = userTOTPSecret;
                        string userAPIKey = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.APIKey = userAPIKey;
                        UserModel.DisplayTheme = 2;
                        _userService.CreateUser(UserModel);

                        //Send email
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);



                        var confirmationLink = Url.Page(
                        "/Account/EmailVerification", pageHandler: null,
                            values: new { userId = identityUser.Id, code = token }, Request.Scheme);

                        // Send the confirmation link to the user's email address
                        // You can use a service like SendGrid or your own email provider.
                        Email email = new Email();
                        email.Subject = "Email Verification";
                        email.Body = "You need to verify here - " + confirmationLink;

                        email.Recipients = new List<string>();
                        email.Recipients.Add(UserModel.Email);
                        email.FirstName = UserModel.FirstName;
                        email.LastName = UserModel.LastName;


                        _emailService.SendEmail(email);
                    }
                }
                else
                {
                    //update an existing user
                    User dbUser = _userService.GetUser(Guid.Parse(UserIDParam));
                    dbUser.FirstName = UserModel.FirstName;
                    dbUser.LastName = UserModel.LastName;
                    dbUser.Email = UserModel.Email;
                    dbUser.Locked = UserModel.Locked;
                    dbUser.IsOrgAdmin = UserModel.IsOrgAdmin;
                    _userService.UpdateUser(Guid.Parse(UserIDParam), dbUser);
                }
                return RedirectToPage("OrganisationDetails", new { id = OrganisationIDParam });
            }
            return Page();
        }
    }
}
