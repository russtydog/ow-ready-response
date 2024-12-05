using MyEF2.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Services
{
    public class AssistantService
    {
        
        public void CreateAssistantFiles(string PublicWebsite,string WebRootPath,string organisationId,string AIBotName,string PrimaryColour)
        {
            

            // Read the contents of trevor.js
            string webRootPath = WebRootPath;
            // Construct the path to the trevor.js file
            string assistantJsPath = Path.Combine(webRootPath, "aiassets", "js", "assistant.js");

            string assistantJsContents = System.IO.File.ReadAllText(assistantJsPath);

            string ChatBotName = AIBotName;
            assistantJsContents = assistantJsContents.Replace("[[ChatbotName]]", ChatBotName);
            assistantJsContents = assistantJsContents.Replace("[[ApplicationURL]]", PublicWebsite);
            assistantJsContents = assistantJsContents.Replace("[[PrimaryColour]]", PrimaryColour);


            assistantJsContents = assistantJsContents.Replace("[[HandlerName]]", "AddMessage");

            // Save the modified contents to a new file
            Directory.CreateDirectory(webRootPath + "/aiassets/js/" + organisationId);
            string saveToPath = Path.Combine(webRootPath, "aiassets", "js", organisationId, "chatbot.js");

            System.IO.File.WriteAllText(saveToPath, assistantJsContents);
        }
        public void UpdateAssistantStyles(string WebRootPath,string PrimaryColour)
        {
            string webRootPath = WebRootPath;
            // Construct the path to the trevor.js file
            string assistantCssPath = Path.Combine(webRootPath, "aiassets", "css", "assistantTemplate.css");
            string assistantCssContents = System.IO.File.ReadAllText(assistantCssPath);
            assistantCssContents = assistantCssContents.Replace("[[PrimaryColour]]",PrimaryColour);
            string saveToPath = Path.Combine(webRootPath, "aiassets", "css", "assistant.css");
            System.IO.File.WriteAllText(saveToPath, assistantCssContents);

            string styleCssPath = Path.Combine(webRootPath, "aiassets", "css", "cb-styleTemplate.css");
            string styleCssContents = System.IO.File.ReadAllText(styleCssPath);
            styleCssContents = styleCssContents.Replace("[[PrimaryColour]]", PrimaryColour);
            string saveToStylePath = Path.Combine(webRootPath, "aiassets", "css", "cb-style.css");
            System.IO.File.WriteAllText(saveToStylePath, styleCssContents);
        }
    }
}
