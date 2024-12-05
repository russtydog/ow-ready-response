using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;
using Saml;
using System.Net;
using System.Security.Claims;

namespace MyEF2.WebApp.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class samlModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserService _userService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly OrganisationService _organisationService;
        private readonly EmailService _emailService;
        private readonly LoginHistoryService _loginHistoryService;


        public samlModel(SettingService settingService, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, UserService userService, OrganisationService organisationService, EmailService emailService,LoginHistoryService loginHistoryService)
        {
            _settingService = settingService;
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _organisationService = organisationService;
            _emailService = emailService;
            _loginHistoryService = loginHistoryService;
        }

        public void OnGet(string? returnUrl = null)
        {
            
        }


        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {

            // Process the SAMLResponse

            Setting settings = _settingService.GetSettings();
            string samlCertificate = settings.SigningCertificate;
            var samlResponse = new Response(samlCertificate, Request.Form["SAMLResponse"]);
            if (samlResponse.IsValid())
            {
                var user = await _userManager.FindByNameAsync(samlResponse.GetEmail());
                if (user != null)
                {
					var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, user.Email), // Add the username claim
                            new Claim(ClaimTypes.Email,user.Email)
						};
					var identity = new ClaimsIdentity(claims, "ApplicationCookie");
					var principal = new ClaimsPrincipal(identity);
					var authProperties = new AuthenticationProperties
					{
						IsPersistent = false,
						ExpiresUtc = DateTime.UtcNow.AddHours(1)
					};
					await HttpContext.SignInAsync("Identity.Application", principal, authProperties);

                    if (!user.EmailConfirmed)
                    {
						await _signInManager.SignOutAsync();
						return RedirectToPage("RegisterConfirmation");
                    }

                    User thisUser = _userService.GetUserByAuthId(user.Email);
                    _loginHistoryService.AddLoginHistory(new DAL.Entities.LoginHistory
                    {
                        Id = Guid.NewGuid(),
                        User = thisUser,
                        Organisation = thisUser.Organisation,
                        LoginTime = DateTime.UtcNow,
                    });

                    if (thisUser.Locked)
                    {
                        return RedirectToPage("AccountLocked");
                    }
                    if(settings.OrganisationsRequireSubscription && !thisUser.Organisation.ActiveSubscription)
                    {
                        //user's organisation has no active subscription
                        return RedirectToPage("NoSubscription");
                    }
                    var decodedReturnURL = WebUtility.UrlDecode("~" + returnUrl);
                    if (decodedReturnURL == "~")
                    {
                        return RedirectToPage("/Index");
                    }
                    return Redirect(decodedReturnURL);
                    
					

				}

				if (settings.EnableAutoRegistration)
                {
                    //if you got here you authenticated with SSO but UserIdentity doesn't exist, so we will create it.
                    var identityUser = new IdentityUser()
                    {

                        UserName = samlResponse.GetEmail(),
                        Email = samlResponse.GetEmail()

                    };
                    var result = _userManager.CreateAsync(identityUser);
                    if (result.Result.Succeeded)
                    {
                        //create a new org for the new user
                        DAL.Entities.User UserModel = new DAL.Entities.User();

                        if (string.IsNullOrEmpty(_settingService.GetSettings().DefaultOrganisationId))
                        {
                            var newOrganisation = new Organisation();
                            newOrganisation.OrganisationName = "My Organisation";
                            newOrganisation.ABN = "";
                            _organisationService.Create(newOrganisation, UserModel.Email);
                            UserModel.Organisation = newOrganisation;
                            UserModel.IsOrgAdmin = true;
                        }
                        else
                        {
                            Organisation org = new Organisation();
                            org = _organisationService.GetOrganisation(Guid.Parse(_settingService.GetSettings().DefaultOrganisationId));
                            UserModel.Organisation = org;
                            UserModel.IsOrgAdmin = false;
                        }

                        UserModel.UserId = new Guid(identityUser.Id);
                        UserModel.Profile = "";
                        UserModel.OTPCode = "";
                        UserModel.TimeZone = "AUS Eastern Standard Time";
                        string userTOTPSecret = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.MFASecret = userTOTPSecret;
                        string apiKey = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.APIKey = apiKey;
                        UserModel.Email=identityUser.Email;
                        UserModel.FirstName=samlResponse.GetFirstName();
                        UserModel.LastName=samlResponse.GetLastName();
                        UserModel.Locked=false;
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
                        email.Body = "Verify your Email address by clicking the button below. ";
                        email.ActionButton = confirmationLink;
                        email.ActionButtonLabel = "Verify Now";
                        email.UseNotificationTemplate = true; 
                   

                        email.Recipients = new List<string>();
                        email.Recipients.Add(UserModel.Email);

                        _emailService.SendEmail(email);
						var decodedReturnURL = WebUtility.UrlDecode("~" + returnUrl);

						return Redirect("RegisterConfirmation");
					}
                }
                if (user == null)
                {
                    return RedirectToPage("Unauthorised");
                }
            }
            
            ModelState.AddModelError("", "An error occurred during SAML handshake or authorization");
            return Page();
        }
    }
}
