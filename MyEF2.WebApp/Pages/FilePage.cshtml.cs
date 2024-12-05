using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Reflection.Metadata;

namespace MyEF2.WebApp.Pages
{
    [Authorize]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class FilePageModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly OrganisationService _organisationService;
        private readonly UserService _userService;
        private IWebHostEnvironment _env;

        public FilePageModel(SettingService settingService, OrganisationService organisationService, UserService userService,IWebHostEnvironment env)
        {
            _settingService = settingService;
            _organisationService = organisationService;
            _userService = userService;
            _env = env;
        }
        public Organisation Organisation { get; set; }
        public List<AIDocument> Documents { get; set; }
        public void OnGet()
        {
            // Add your logic here for handling the GET request
            var user = _userService.GetUserByAuthId(User.Identity.Name);
            Organisation= _organisationService.GetOrganisation(user.Organisation.Id,user.Email);
            Documents = Organisation.AIDocuments;
        }

        public void OnPost()
        {
            // Add your logic here for handling the POST request
        }
        public async Task<IActionResult> OnPostUploadAsync(string? id)
        {
            Setting settings = _settingService.GetSettings();
            var thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            var organisation = _organisationService.GetOrganisation(thisUser.Organisation.Id);

            var files = Request.Form.Files;
            foreach (var file in files)
            {
                if (file != null)
                {
                    // Get the file name with extension
                    var fileName = Path.GetFileName(file.FileName);

                    //remove special characters from fileName so it can be copied successfully
                    fileName = fileName.Replace("&", " ");
                    fileName = fileName.Replace("-", " ");
                    fileName = fileName.Replace("+", " ");
                    fileName = fileName.Replace("-", " ");

                    // Set the path where the file will be saved
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                    //get web relative path
                    var relativePath = filePath.Replace(Directory.GetCurrentDirectory() + @"\wwwroot", "");

                    
                    // Create a new file stream
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        // Copy the uploaded file to the new file stream
                        await file.CopyToAsync(fileStream);
                    }

                    //Upload file to OpenAI
                    string selectedFilePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                    
                    
                    DAL.Entities.AIDocument document = new DAL.Entities.AIDocument
                    {
                        DocumentName = fileName,
                        DocumentPath = relativePath,
                        FileId= ""
                    };
                    _organisationService.UpsertFile(thisUser.Organisation.Id, document);

                    // Upload file to OpenAI

                    // Create the JSON response
                    var response = new
                    {
                        status = "success",
                        info = "File uploaded successfully",
                        file_link = "/uploads/" + fileName
                    };

                    //serialise response to json string using newtonsoft json
                    var json = JsonConvert.SerializeObject(response);

                    return Content(json, "application/json");
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostRemoveAsync(string? id, string? fileName)
        {
            var thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            AIDocument document = _organisationService.GetDocument(thisUser.Organisation.Id, fileName);

            _organisationService.RemoveFile(thisUser.Organisation.Id, document);

            Setting settings = _settingService.GetSettings();


            return Page();
        }
    }
}