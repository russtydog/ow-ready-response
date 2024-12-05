using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;
using Saml;
using System.Net;
using System.Security.Claims;

namespace MyEF2.WebApp.Pages.SAML
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly OrganisationService _organisationService;
        private readonly SettingService _settingService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserService _userService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly LoginHistoryService _loginHistoryService;


        public IndexModel(OrganisationService organisationService,SettingService settingService,UserManager<IdentityUser> userManager,UserService userService,EmailService emailService,SignInManager<IdentityUser> signInManager,LoginHistoryService loginHistoryService)
        {
            _organisationService = organisationService;
            _settingService = settingService;
            _userManager = userManager;
            _userService = userService;
            _signInManager=signInManager;
            _emailService=emailService;
            _loginHistoryService=loginHistoryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id,string? returnUrl = null)
        {
            if(id!=null)
            {
                Id = Guid.Parse(id);
                DAL.Entities.Organisation organisation = _organisationService.GetOrganisation(Guid.Parse(id));

                var samlEndpoint = organisation.LoginURL;

                string strReturnURL = returnUrl != null ? "?ReturnUrl=" + returnUrl : "";

                string url = Request.Scheme + "://" + Request.Host;
                string redir = url + "/SAML/" + organisation.Id.ToString();

            
                var request = new AuthRequest(
                    organisation.EntityID,
                    redir + strReturnURL
                    );

                return Redirect(request.GetRedirectUrl(samlEndpoint));
            }

            return Page();
        }

        public async Task<IActionResult> OnPost(string? returnUrl = null)
        {

            //Get the OrganisationId from the URL
            string orgId = Request.Path.Value.Split("/")[2];
            DAL.Entities.Organisation organisation = _organisationService.GetOrganisation(Guid.Parse(orgId));

            Setting settings = _settingService.GetSettings();
            string samlCertificate = organisation.SigningCertificate;
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
                        return RedirectToPage("../Account/RegisterConfirmation");
                    }

                    

                    User thisUser = _userService.GetUserByAuthId(user.Email);

                    // _loginHistoryService.AddLoginHistory(new DAL.Entities.LoginHistory
                    // {
                    //     Id = Guid.NewGuid(),
                    //     User = thisUser,
                    //     Organisation = thisUser.Organisation,
                    //     LoginTime = DateTime.UtcNow,
                    // });

                    // get GetUserLoginHistory which returns List<LoginHistory> and find the last login event to get the ReturnUrl
                    var loginHistory = _loginHistoryService.GetUserLoginHistory(thisUser).OrderBy(x=>x.LoginTime).LastOrDefault();
                    
                    if (thisUser.Locked)
                    {
                        return RedirectToPage("../Account/AccountLocked");
                    }
                    
                    // var decodedReturnURL = WebUtility.UrlDecode("~" + returnUrl);
                    // if (decodedReturnURL == "~")
                    // {
                    //     return RedirectToPage("../Dashboard");
                    // }
                    // return Redirect(decodedReturnURL);

                    if (loginHistory != null && loginHistory.ReturnUrl != null)
                    {
                        var decodedReturnURL = WebUtility.UrlDecode("~" + loginHistory.ReturnUrl);
                        if (decodedReturnURL == "~")
                        {
                            return RedirectToPage("../Dashboard");
                        }
                        return Redirect(decodedReturnURL);
                    }
                    else
                    {
                        return RedirectToPage("../Dashboard");
                    }


                }

                if (organisation.EnableSSO && organisation.EnableAutoSSORegistration)
                {
                    //if you got here you authenticated with SSO but UserIdentity doesn't exist, so we will create it.
                    var skipEmailVerification = organisation.SkipEmailVerification;
                    var identityUser = new IdentityUser()
                    {

                        UserName = samlResponse.GetEmail(),
                        Email = samlResponse.GetEmail(),
                        EmailConfirmed = skipEmailVerification

                    };
                    var result = _userManager.CreateAsync(identityUser);
                    if (result.Result.Succeeded)
                    {
                        //create a new org for the new user
                        DAL.Entities.User UserModel = new DAL.Entities.User();

                        
                        UserModel.Organisation = organisation;
                        UserModel.IsOrgAdmin = false;

                        UserModel.UserId = new Guid(identityUser.Id);
                        UserModel.Profile = "";
                        UserModel.OTPCode = "";
                        UserModel.TimeZone = "AUS Eastern Standard Time";
                        string userTOTPSecret = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.MFASecret = userTOTPSecret;
                        string apiKey = TOTPGenerator.GenerateTOTPSecret();
                        UserModel.APIKey = apiKey;
                        UserModel.Email = identityUser.Email;
                        UserModel.FirstName = samlResponse.GetFirstName();
                        UserModel.LastName = samlResponse.GetLastName();
                        UserModel.Locked = false;
                        UserModel.DisplayTheme = 2;

                        _userService.CreateUser(UserModel);

                        //Send email
                        if (skipEmailVerification)
                        {
							//should re-sso
							var userSkip = await _userManager.FindByNameAsync(samlResponse.GetEmail());

							var claims = new List<Claim>
						    {
							    new Claim(ClaimTypes.Name, userSkip.Email), // Add the username claim
                                new Claim(ClaimTypes.Email,userSkip.Email)
						    };
							var identity = new ClaimsIdentity(claims, "ApplicationCookie");
							var principal = new ClaimsPrincipal(identity);
							var authProperties = new AuthenticationProperties
							{
								IsPersistent = false,
								ExpiresUtc = DateTime.UtcNow.AddHours(1)
							};
							await HttpContext.SignInAsync("Identity.Application", principal, authProperties);
							var decodedReturnURLCheck = WebUtility.UrlDecode("~" + returnUrl);
							if (decodedReturnURLCheck == "~")
							{
								return RedirectToPage("../Dashboard");
							}
							return Redirect(decodedReturnURLCheck);

						}
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);



                        var confirmationLink = Url.Page(
                        "/Account/EmailVerification", pageHandler: null,
                            values: new { userId = identityUser.Id, code = token }, Request.Scheme);

                        // Send the confirmation link to the user's email address
                        // You can use a service like SendGrid or your own email provider.
                        Email email = new Email();
                        email.Subject = "Email Verification";
                        email.Body = "Please verify your email address by clicking the button below";
                        email.ActionButton = confirmationLink;
                        email.ActionButtonLabel = "Verify Now";
                        email.UseNotificationTemplate = true;

                        email.Recipients = new List<string>();
                        email.Recipients.Add(UserModel.Email);

                        _emailService.SendEmail(email);
                        var decodedReturnURL = WebUtility.UrlDecode("~" + returnUrl);

                        return Redirect("../Account/RegisterConfirmation");
                    }
                }
                if(user==null)
                {
                    return RedirectToPage("../Account/Unauthorised");
                }
            }
            ModelState.AddModelError("", "An error occurred during SAML handshake or authorization");
            return Page();
        }
    }
}
