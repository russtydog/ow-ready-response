using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Net.Codecrete.QrCodeGenerator;

namespace MyEF2.WebApp.Pages.Account
{
    [Authorize]
    public class MFAStep1Model : PageModel
    {
        private readonly UserService _userService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        [BindProperty]
        public string ImgSrc { get; set; }
        [BindProperty]
        public string MFASecret { get; set; }   
        public MFAStep1Model(UserService userService, IWebHostEnvironment hostingEnvironment)
        {
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
        }
        public void OnGet()
        {
            DAL.Entities.User user = _userService.GetUserByAuthId(User.Identity.Name);

            if (String.IsNullOrEmpty(user.MFASecret) || user.MFASecret == "")
            {
                string userTOTPSecret = TOTPGenerator.GenerateTOTPSecret();
                user.MFASecret = userTOTPSecret;
            }

            // Replace with your TOTP secret
            string totpSecret = user.MFASecret;

            // Generate the TOTP URI for the QR code
            string totpUri = $"otpauth://totp/MyEF2:" + user.Email + "?secret=" + totpSecret + "&issuer=MyEF2";

            var qr = QrCode.EncodeText(totpUri, QrCode.Ecc.Medium);
            string svg = qr.ToSvgString(4);
            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "qrcode.svg");
            System.IO.File.WriteAllText(filePath, svg, Encoding.UTF8);
            ImgSrc = "/uploads/qrcode.svg";
            MFASecret =user.MFASecret;
        }
        public IActionResult OnPost()
        {
            ModelState.Remove("ImgSrc");
            if(ModelState.IsValid)
            {
                DAL.Entities.User user = _userService.GetUserByAuthId(User.Identity.Name);
                user.MFASecret = MFASecret; //ensures we use the scanned code rather than the db value as it may have been generated onload
                _userService.UpdateUser(user.Id, user);
                return RedirectToPage("MFAStep2"); //go to Step2 to test the code, if it works then we can enable MFA
            }
            return Page();
        }
        

    }
}
