using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using System.Net;
using System.Security.Claims;
using Net.Codecrete.QrCodeGenerator;
using System.Text;

namespace MyEF2.WebApp.Pages.Account
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class DeviceLoginModel : PageModel
    {
        private readonly DeviceLoginRequestService _deviceLoginRequestService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserService _userService;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly LoginHistoryService _loginHistoryService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DeviceLoginModel(DeviceLoginRequestService deviceLoginRequestService,UserManager<IdentityUser> userManager,UserService userService, SignInManager<IdentityUser> signInManager,LoginHistoryService loginHistoryService,IWebHostEnvironment hostingEnvironment)
        {
            _deviceLoginRequestService = deviceLoginRequestService;
            _userManager = userManager;
            _userService = userService;
            _signInManager = signInManager;
            _loginHistoryService = loginHistoryService;
            _hostingEnvironment = hostingEnvironment;
        }
        [BindProperty]
        public string LoginCode { get; set; }
        public string QRCode { get; set; }
        public void OnGet()
        {
            //generate a 6 digit random code
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            LoginCode = result;

            // Generate the TOTP URI for the QR code
            string deviceLoginUrl = $"https://{Request.Host}/Account/LoginRemoteDevice/" + LoginCode;

            var qr = QrCode.EncodeText(deviceLoginUrl, QrCode.Ecc.Medium);
            string svg = qr.ToSvgString(4);
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "devicelogin" + LoginCode + ".svg");
            System.IO.File.WriteAllText(filePath, svg, Encoding.UTF8);
            QRCode = "/uploads/devicelogin" + LoginCode + ".svg";

            //now add the LoginCode to the DeviceLoginRequest table
            var deviceLoginRequest = _deviceLoginRequestService.AddDeviceLoginRequest(new DeviceLoginRequest
            {
                LoginCode = LoginCode
            });
        }
        public async Task<IActionResult> OnPostRequestCheckAsync(string? loginCode)
        {
            //var loginCode = Request.Form["loginCode"];
            var deviceLoginRequest = _deviceLoginRequestService.GetDeviceLoginRequest(loginCode);
            if (deviceLoginRequest != null)
            {
                //Return Json success with the deviceLoginRequest
                return new JsonResult(deviceLoginRequest);
            }
            return null;
        }
        public async Task<IActionResult> OnPostProcessAuthAsync(string? id)
        {
            var deviceLoginRequest = _deviceLoginRequestService.GetDeviceLoginRequest(true,id);
            if (deviceLoginRequest != null)
            {
                //delete the deviceLoginRequest
                _deviceLoginRequestService.DeleteDeviceLoginRequest(deviceLoginRequest.Id);

                if(deviceLoginRequest.UserEmail==null)
                {
                    return new JsonResult(new { success = false, returntopage = "Login" });
                }
                //the AuthenticationCode is the user Email, therefore set the User.Identity.User to this
                var user = await _userManager.FindByNameAsync(deviceLoginRequest.UserEmail);
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
                        //return RedirectToPage("RegisterConfirmation");
                        return new JsonResult(new { success = false, returntopage = "RegisterConfirmation" });

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
                        //return RedirectToPage("AccountLocked");
                        return new JsonResult(new { success = false, returntopage = "AccountLocked" });
                    }
                    
                    return new JsonResult(new { success = true, returntopage = "/Dashboard" });
                    
                }
            }
            return new JsonResult(new { success = false, returntopage = "Login" });
        }
    }
}
