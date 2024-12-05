using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using MyEF2.DAL.Services;
using MyEF2.DAL.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using HtmlAgilityPack;

namespace MyEF2.WebApp.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly UserService _userService;

        public DashboardModel(SettingService settingService, UserService userService)
        {
            _settingService = settingService;
            _userService = userService;
        }
        public DAL.Entities.User CurrentUser { get; set; }
        public void OnGet()
        {
            CurrentUser = _userService.GetUserByAuthId(User.Identity.Name);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUploadFile()
        {
            var file = Request.Form.Files[0];
            //add a new folder with a guid
            var folderName = Guid.NewGuid().ToString();

            //create the guid folder
            var folderPath = Path.Combine("wwwroot/uploads", folderName);
            System.IO.Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine("wwwroot/uploads",folderName, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Append))
            {
                await file.CopyToAsync(stream);
            }
            HttpContext.Session.SetString("uploadedFilePath", filePath);


            return new JsonResult(new { success = true });
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostStartProcessingQuestion([FromBody] QuestionModel model)
        {
            var user = _userService.GetUserByAuthId(User.Identity.Name);
            var client = new RestClient(user.Organisation.AskAIAPI);
            var request = new RestRequest("api/chat", Method.Post);
            request.AddHeader("Authorization", user.Organisation.AskAIAPIKey); // Replace with your actual API key
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new { 
                
                messages="",
                selectedOption="",
                apiKey=user.Organisation.AskAIAssistantId,
                conversationId=model.ConversationId,
                lastMessage = model.Question,
                ignoreContent = false,
                
                });

            //// APIResponse isn't correct, it should just have a MessageId, its the response of the next method which will have the status
            Console.WriteLine("Sending message to API");
            var response = await client.ExecuteAsync<ApiResponse>(request);

            if (response.Data != null && !string.IsNullOrEmpty(response.Data.MessageId))
            {
                return new JsonResult(new { messageId = response.Data.MessageId });
            }

            return new JsonResult(new { messageId = (string)null });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnGetPollForResponse(string messageId, int rowIndex)
        {
            var user = _userService.GetUserByAuthId(User.Identity.Name);
            var client = new RestClient(user.Organisation.AskAIAPI);
            var request = new RestRequest($"api/chat/{messageId}", Method.Get);
            request.AddHeader("Authorization", user.Organisation.AskAIAPIKey); // Replace with your actual API key

            var response = await client.ExecuteAsync<ApiResponse>(request);

            if (response.Data != null)
            {
                Console.WriteLine("Polling for response");
                return new JsonResult(new
                {
                    streamStatus = response.Data.StreamStatus,
                    messageStream = response.Data.MessageStream
                });
            }
            if (response.Content != null && response.Content.Contains("messageId not found"))
            {
                return new JsonResult(new { streamStatus = "Preparing", messageStream = "Preparing" });
            }

            return new JsonResult(new { streamStatus = "Error", messageStream = "Error" });
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostSaveResponses([FromBody] SaveResponsesRequest request)
        {
            // Load the spreadsheet using the stored file path
            var filePath = HttpContext.Session.GetString("uploadedFilePath");
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("File path not found.");

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var worksheet = package.Workbook.Worksheets[0];

                // Write responses to Column D
                for (int i = 0; i < request.Responses.Count; i++)
                {
                    var cell = worksheet.Cells[i + 2, 4]; // Assuming the first row is a header
                    var htmlContent = request.Responses[i];

                    // Parse the HTML content and add it to the cell as RichText
                    var richText = cell.RichText;
                    ParseHtmlContent(htmlContent, richText);
                }

                // Save the updated spreadsheet
                package.Save();

                // Return a success message
                return new JsonResult(new { success = true });
            }
        }

        private void ParseHtmlContent(string htmlContent, ExcelRichTextCollection richText)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);

            foreach (var node in doc.DocumentNode.DescendantsAndSelf())
            {
                switch (node.Name)
                {
                    case "#text":
                        richText.Add(node.InnerText);
                        break;
                    case "b":
                    case "strong":
                        var boldText = richText.Add(node.InnerText);
                        boldText.Bold = true;
                        break;
                    case "i":
                    case "em":
                        var italicText = richText.Add(node.InnerText);
                        italicText.Italic = true;
                        break;
                    case "u":
                        var underlineText = richText.Add(node.InnerText);
                        underlineText.UnderLine = true;
                        break;
                    case "p":
                        richText.Add("\n" + node.InnerText + "\n");
                        break;
                    case "ul":
                    case "ol":
                        foreach (var li in node.ChildNodes.Where(n => n.Name == "li"))
                        {
                            richText.Add("\n• " + li.InnerText);
                        }
                        break;
                    case "li":
                        richText.Add("\n• " + node.InnerText);
                        break;
                }
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnGetDownloadSpreadsheet()
        {
            // Load the spreadsheet using the stored file path
            var filePath = HttpContext.Session.GetString("uploadedFilePath");
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("File path not found.");

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                package.SaveAs(stream);
            }
            stream.Position = 0;

            // Return the updated spreadsheet as a file download
            // get the original filename excluding the extension
            var fileName = Path.GetFileNameWithoutExtension(filePath);


            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + "_updated.xlsx");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnGetGetUploadedFilePath()
        {
            var filePath = HttpContext.Session.GetString("uploadedFilePath");
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("File path not found.");

            return new JsonResult(new { filePath = filePath });
        }

        public class QuestionModel
        {
            public string Question { get; set; }
            public int RowIndex { get; set; }
            public string ConversationId{get;set;}
        }

        public class ApiResponse
        {
            public string MessageId { get; set; }
            public string StreamStatus { get; set; }
            public string MessageStream { get; set; }
        }
        public class SaveResponsesRequest
        {
            public List<string> Responses { get; set; }
        }
    }
}
