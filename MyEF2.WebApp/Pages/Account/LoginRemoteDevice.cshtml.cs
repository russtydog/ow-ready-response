using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Services;
using Microsoft.AspNetCore.Authorization;

namespace MyEF2.WebApp.Pages.Account
{
    [Authorize]
    public class LoginRemoteDeviceModel : PageModel
    {
        private readonly DeviceLoginRequestService _deviceLoginRequestService;
        public LoginRemoteDeviceModel(DeviceLoginRequestService deviceLoginRequestService)
        {
            _deviceLoginRequestService = deviceLoginRequestService;
        }
        [BindProperty]
        public string LoginCode { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id != null)
            {
                //there is a code in the URI so we can see if we can find it
                var deviceLoginRequest = _deviceLoginRequestService.GetDeviceLoginRequest(id);
                if (deviceLoginRequest != null)
                {
                    var random = new Random();
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var result = new string(Enumerable.Repeat(chars, 18)
                    .Select(s => s[random.Next(s.Length)]).ToArray());


                    deviceLoginRequest.AuthenticationCode = result;
                    deviceLoginRequest.UserEmail = User.Identity.Name;
                    _deviceLoginRequestService.UpdateDeviceLoginRequest(deviceLoginRequest);
                    return RedirectToPage("Profile");
                }
                ModelState.AddModelError("LoginCode", "Invalid Login Code");
                return Page();

            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var deviceLoginRequest = _deviceLoginRequestService.GetDeviceLoginRequest(LoginCode);
            if (deviceLoginRequest != null)
            {
                var random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var result = new string(Enumerable.Repeat(chars, 18)
                  .Select(s => s[random.Next(s.Length)]).ToArray());


                deviceLoginRequest.AuthenticationCode = result;
                deviceLoginRequest.UserEmail = User.Identity.Name;
                _deviceLoginRequestService.UpdateDeviceLoginRequest(deviceLoginRequest);
                return RedirectToPage("Profile");
            }
            ModelState.AddModelError("LoginCode", "Invalid Login Code");
            return Page();
        }
        
    }
}
