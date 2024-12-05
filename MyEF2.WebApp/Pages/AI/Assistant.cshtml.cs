using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Newtonsoft.Json.Linq;
using RestSharp;
using Stripe.Tax;
using System.Reflection.Metadata;

namespace MyEF2.WebApp.Pages.AI
{
    [Authorize]  
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class AssistantModel : PageModel
    {
        private readonly OrganisationService _organisationService;
        private readonly UserService _userService;
        private IWebHostEnvironment _env;
        private readonly SettingService _settingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssistantModel(OrganisationService organisationService, UserService userService, IWebHostEnvironment env, SettingService settingService, IHttpContextAccessor httpContextAccessor)
        {
            _organisationService = organisationService;
            _userService = userService;
            _env = env;
            _settingService = settingService;
            _httpContextAccessor = httpContextAccessor;
        }
        [BindProperty]
        public Organisation Organisation { get; set; }
        [BindProperty]
        public string PublicWebsite { get; set; }
        public void OnGet()
        {
            var thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            Organisation= _organisationService.GetOrganisation(thisUser.Organisation.Id,thisUser.Email);
            Organisation.AIAssistantId = string.IsNullOrEmpty(Organisation.AIAssistantId)?"":Encryption.Decrypt(Organisation.AIAssistantId);

            var request = _httpContextAccessor.HttpContext.Request;
            string host = request.Host.Host;
            string port = request.Host.Port.HasValue ? ":" + request.Host.Port : "";
            PublicWebsite = request.Scheme + "://" + host + port;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            var organisation = _organisationService.GetOrganisation(thisUser.Organisation.Id);
            organisation.AssistantName = Organisation.AssistantName;
            organisation.AIInstructions= Organisation.AIInstructions;
            organisation.AIAssistantTargetWebsite= Organisation.AIAssistantTargetWebsite;
            organisation.ShowAssistant=Organisation.ShowAssistant;
            Setting settings = _settingService.GetSettings();
            OpenAIService openAIService = new OpenAIService(_organisationService);

            var action = Request.Form["action"];
            if (action == "delete")
            {
                
                openAIService.DeleteAssistant(organisation, Encryption.Decrypt(settings.OpenAIKey));

                organisation.AIAssistantId = "";
                organisation.AssistantName= "";
                organisation.AIInstructions = "";
                organisation.AIAssistantTargetWebsite = "";
                organisation.AIDocuments = null;
                organisation.VectorStoreId = null;
                _organisationService.UpdateOrganisations(organisation, User.Identity.Name);
                return RedirectToPage("/Index");
            }

            //Upload files to OpenAI
            //get files in a List<string>
            List<string> fileIds = new List<string>();
            List<AIDocument> documents = organisation.AIDocuments;
            foreach (AIDocument document in documents)
            {
                if (!string.IsNullOrEmpty(document.FileId))
                {
                    fileIds.Add(document.FileId);
                }
            }

            var assistantId = string.IsNullOrEmpty(organisation.AIAssistantId)==true?null:Encryption.Decrypt(organisation.AIAssistantId).ToString();
            var vectorStoreId = string.IsNullOrEmpty(organisation.VectorStoreId) == true ? null : Encryption.Decrypt(organisation.VectorStoreId).ToString();

            var createdassistantId  = openAIService.CreateAssistant(assistantId, Encryption.Decrypt(settings.OpenAIKey),organisation.AssistantName,organisation.AIInstructions, fileIds, vectorStoreId);
            if(createdassistantId!=""){
                organisation.AIAssistantId = Encryption.Encrypt(createdassistantId);
            }
            //create the js files
            AssistantService assistantService = new AssistantService();
            var request = _httpContextAccessor.HttpContext.Request;
            string host = request.Host.Host;
            string port = request.Host.Port.HasValue ? ":" + request.Host.Port : "";
            string PublicWebsite = request.Scheme + "://" + host + port;
            assistantService.CreateAssistantFiles(PublicWebsite, _env.WebRootPath, organisation.Id.ToString(), organisation.AssistantName,settings.ActiveItemBackgroundColor);

            assistantService.UpdateAssistantStyles(_env.WebRootPath, settings.ActiveItemBackgroundColor);

            _organisationService.UpdateOrganisations(organisation, User.Identity.Name);
            return RedirectToPage("/AI/Assistant");
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
                    
                    OpenAIService openAIService = new OpenAIService(_organisationService);
                    var uploadedFileId = openAIService.AddFile(selectedFilePath, Encryption.Decrypt(settings.OpenAIKey), organisation, User.Identity.Name);

                    DAL.Entities.AIDocument document = new DAL.Entities.AIDocument
                    {
                        DocumentName = fileName,
                        DocumentPath = relativePath,
                        FileId= uploadedFileId
                    };
                    _organisationService.UpsertFile(thisUser.Organisation.Id, document);
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

            OpenAIService openAIService = new OpenAIService(_organisationService);
            openAIService.RemoveFile(document.FileId, Encryption.Decrypt(settings.OpenAIKey),Encryption.Decrypt(thisUser.Organisation.VectorStoreId));

            return Page();
        }
    }
}
