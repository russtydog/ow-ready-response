using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Reflection.Metadata;
using MyEF2.DAL.Models;
using Markdig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MyEF2.DAL.DatabaseContexts;
using System.Xml;
using MyEF2.DAL.Migrations;
using Microsoft.AspNetCore.Cors;

namespace MyEF2.WebApp.Pages.AI
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class ChatModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly OrganisationService _organisationService;
        private readonly ConversationService _conversationService;
        private readonly MessageService _messageService;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public ChatModel(SettingService settingService, OrganisationService organisationService, ConversationService conversationService, MessageService messageService, IServiceScopeFactory serviceScopeFactory)
        {
            _settingService = settingService;
            _organisationService = organisationService;
            _conversationService = conversationService;
            _messageService = messageService;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public void OnGet()
        {
        }
        [EnableCors("AllowAll")]

        public async Task<IActionResult> OnPostAddMessageAsync([FromBody] AssistantRequest data)
        {
            Setting setting = _settingService.GetSettings();

            var resp = data.SelectedOption;

            var referer = Request.Headers["Referer"].ToString();
            Organisation organisation = _organisationService.GetOrganisation(Guid.Parse(data.ApiKey));
            if (organisation == null)
            {
                return new JsonResult(new AssistantResponse { Message = "Invalid request", MessageId = "" });
            }
            if (string.IsNullOrEmpty(organisation.AIAssistantTargetWebsite))
            {
                return new JsonResult(new AssistantResponse { Message = "Invalid request", MessageId = "" });
            }

            //url of this website
            var thisWebsite = "https://" + Request.Host.Value;

            if (!referer.Contains(organisation.AIAssistantTargetWebsite) && !referer.Contains(thisWebsite))
            {
                return new JsonResult(new AssistantResponse { Message = "Invalid request: " + organisation.AssistantName + " is not allowed to chat on this website.", MessageId = "" });
            }



            Conversation conversation = _conversationService.GetConversation(Guid.Parse(data.ConversationId));
            if (conversation == null)
            {
                conversation = new Conversation();
                conversation.Id = Guid.Parse(data.ConversationId);
                conversation.Started = DateTime.UtcNow;
                conversation.Organisation = organisation;
                conversation = _conversationService.Create(conversation);
            }

            //log request message
            Message message = new Message();
            message.Created = DateTime.UtcNow;
            message.Sender = data.IgnoreContent ? "Prompt" : "User";
            message.Response = data.LastMessage;
            message.Conversation = conversation;
            message.Tokens = 0;
            var userMessage = _messageService.Create(message);

            OpenAIService aiService = new OpenAIService(_organisationService, _conversationService, _messageService, _serviceScopeFactory);
            ConversationMessageStarterResponse response = aiService.CreateConversation(conversation, Encryption.Decrypt(setting.OpenAIKey), data, organisation, "https://" + Request.Host.Value, _messageService, userMessage);


            return new JsonResult(new AssistantResponse { Message = "", MessageId = response.MessageId });
        }
        [EnableCors("AllowAll")]
        public async Task<IActionResult> OnPostMessageStreamAsync(string MessageId)
        {

            var message = _messageService.GetMessage(Guid.Parse(MessageId));
            if (message == null)
            {
                return new JsonResult(new { Response = "Invalid request: message not found" });
            }

            var finalResponse = "";
            string htmlText = "";
            string htmlStreamText = "";
            string messageStream = message.MessageStream;

            //it's in progress so perhaps we can return the message stream but do the formatting.
            var parts = messageStream.Split(new string[] { "```" }, StringSplitOptions.None); //just the in progress MessageStream value
            bool isCodeBlock = false; // Toggle to keep track of alternating code and text blocks


            foreach (var part in parts)
            {
                if (isCodeBlock)
                {
                    // For code blocks, encode and wrap in <pre><code>
                    var encodedCode = System.Net.WebUtility.HtmlEncode(part);
                    finalResponse += "<pre><code>" + encodedCode + "</code></pre>";
                }
                else
                {
                    // For text, render as normal text. Ensure to encode to prevent XSS.
                    var encodedText = System.Net.WebUtility.HtmlEncode(part);
                    finalResponse += encodedText;
                }
                isCodeBlock = !isCodeBlock; // Toggle between code and text blocks
            }
            string markdownText = finalResponse; // Your Markdown text
            htmlStreamText = Markdown.ToHtml(markdownText); // Convert to HTML
            messageStream = htmlStreamText;

            if (message.StreamStatus == "Complete")
            {
                htmlText = messageStream;


            }
            //}

            return new JsonResult(new { MessageStream = messageStream, StreamStatus = message.StreamStatus, FinalResponse = htmlText });
        }
        [EnableCors("AllowAll")]
        public async Task<IActionResult> OnPostHistoryAsync([FromBody] HistoryRequest data)
        {
            var referer = Request.Headers["Referer"].ToString();
            Organisation organisation = _organisationService.GetOrganisation(Guid.Parse(data.ApiKey));
            if (organisation == null)
            {
                return new JsonResult(new { Response = "Invalid request, API Key could be incorrect" });
            }
            if (string.IsNullOrEmpty(organisation.AIAssistantTargetWebsite))
            {
                return new JsonResult(new { Response = "Invalid request, Requesting website not allowed" });
            }

            var thisWebsite = "https://" + Request.Host.Value;

            if (!referer.Contains(organisation.AIAssistantTargetWebsite) && !referer.Contains(thisWebsite))
            {
                return new JsonResult(new { Response = "Invalid request: " + organisation.AssistantName + " is not allowed to chat on this website." });
            }

            var conversation = _conversationService.GetConversation(Guid.Parse(data.ConversationId));

            if (conversation == null)
            {
                return new JsonResult(new { Response = "Invalid request: conversation history not found" });
            }

            //foreach conversaion.Messages, add to List<HistoryResponse>
            var messages = new List<HistoryResponse>();
            foreach (var message in conversation.Messages)
            {
                if (message.Sender == "User")
                {
                    messages.Add(new HistoryResponse { Class = "mymessages", Message = message.Response, MessageId = message.Id.ToString() });
                }
                if (message.Sender == "Prompt")
                {
                    messages.Add(new HistoryResponse { Class = "hiddenprompt", Message = message.Response, MessageId = message.Id.ToString() });
                }

                if (message.Sender == "AI")
                {
                    var parts = message.Response.Split(new string[] { "```" }, StringSplitOptions.None);
                    bool isCodeBlock = false; // Toggle to keep track of alternating code and text blocks

                    var finalResponse = "";
                    foreach (var part in parts)
                    {
                        if (isCodeBlock)
                        {
                            // For code blocks, encode and wrap in <pre><code>
                            var encodedCode = System.Net.WebUtility.HtmlEncode(part);
                            finalResponse += "<pre><code>" + encodedCode + "</code></pre>";
                        }
                        else
                        {
                            // For text, render as normal text. Ensure to encode to prevent XSS.
                            var encodedText = System.Net.WebUtility.HtmlEncode(part);
                            finalResponse += encodedText;
                        }
                        isCodeBlock = !isCodeBlock; // Toggle between code and text blocks
                    }

                    string markdownFinalText = finalResponse; // Your Markdown text
                    var finalResponseHtml = Markdown.ToHtml(markdownFinalText); // Convert to HTML



                    messages.Add(new HistoryResponse { Class = "trevormessages", Message = finalResponseHtml, MessageId = message.Id.ToString() });
                }

            }

            return new JsonResult(new { Response = messages });
        }
        public class StreamServiceMessageRequest
        {
            public string MessageId { get; set; }
        }
    }
}
