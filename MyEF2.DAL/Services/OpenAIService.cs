using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace MyEF2.DAL.Services
{
    public class OpenAIService
    {
        private readonly OrganisationService _organisationService;
        private readonly ConversationService _conversationService;
        private readonly MessageService _messageService;
        private readonly IServiceScopeFactory _serviceScopeFactory;



        public OpenAIService(OrganisationService organisationService)
        {
            _organisationService = organisationService;
        }

        public OpenAIService(OrganisationService organisationService, ConversationService conversationService, MessageService messageService, IServiceScopeFactory serviceScopeFactory)
        {
            _organisationService = organisationService;
            _conversationService = conversationService;
            _messageService = messageService;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public string AddFile(string SelectedFilePath, string OpenAIKey, Organisation organisation, string UserName)
        {
            var client = new RestClient("https://api.openai.com/v1/");
            var request = new RestRequest("files", RestSharp.Method.Post);
            request.AddHeader("Authorization", "Bearer " + OpenAIKey);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("file", SelectedFilePath);
            request.AddParameter("purpose", "assistants", ParameterType.GetOrPost);

            var fileResponse = client.Execute(request);
            var fileResponseJson = JObject.Parse(fileResponse.Content);
            var uploadedFileId = "";
            if (fileResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Create a new document with the file id

                uploadedFileId = fileResponseJson["id"].ToString();
            }

            //if organisation doesn't have a vectorstore, create it
            if (string.IsNullOrEmpty(organisation.VectorStoreId))
            {
                var clientStore = new RestClient("https://api.openai.com/v1/");
                var requestStores = new RestRequest("vector_stores", RestSharp.Method.Post);
                requestStores.AddHeader("Authorization", "Bearer " + OpenAIKey);
                requestStores.AddHeader("Content-Type", " application/json");
                requestStores.AddHeader("OpenAI-Beta", "assistants=v2");
                requestStores.AddJsonBody(new
                {
                    name = organisation.AssistantName + " Vector Store"
                });

                var vectorResponse = clientStore.Execute(requestStores);
                var vectorResponseJson = JObject.Parse(vectorResponse.Content);
                organisation.VectorStoreId = Encryption.Encrypt(vectorResponseJson["id"].ToString());
                _organisationService.UpdateOrganisations(organisation, UserName);
            }

            var vectorStore = Encryption.Decrypt(organisation.VectorStoreId);

            //add file to vector store
            var requestVectorStoreFile = new RestRequest("vector_stores/" + vectorStore + "/files", RestSharp.Method.Post);
            requestVectorStoreFile.AddHeader("Authorization", "Bearer " + OpenAIKey);
            requestVectorStoreFile.AddHeader("Content-Type", " application/json");
            requestVectorStoreFile.AddHeader("OpenAI-Beta", "assistants=v2");
            requestVectorStoreFile.AddJsonBody(new
            {
                file_id = uploadedFileId
            });

            var vectorStoreFileResponse = client.Execute(requestVectorStoreFile);

            return uploadedFileId;

        }
        public string RemoveFile(string FileId, string OpenAIKey, string VectorStoreId)
        {
            var client = new RestClient("https://api.openai.com/v1/");
            try
            {

                //remove file from vector
                var requestRemoveVectorFile = new RestRequest("vector_stores/" + VectorStoreId + "/files/" + FileId, RestSharp.Method.Delete);
                requestRemoveVectorFile.AddHeader("Authorization", "Bearer " + OpenAIKey);
                requestRemoveVectorFile.AddHeader("Content-Type", " application/json");
                requestRemoveVectorFile.AddHeader("OpenAI-Beta", "assistants=v2");
                var vectorDeleteResponse = client.Execute(requestRemoveVectorFile);

                //remove file from storage
                var request = new RestRequest("files/" + FileId, RestSharp.Method.Delete);
                request.AddHeader("Authorization", "Bearer " + OpenAIKey);
                var fileResponse = client.Execute(request);


                return "File Removed";
            }
            catch (Exception ex)
            {
                //Log the error
                return "Error Deleting File";
            }
        }
        public string DeleteAssistant(Organisation organisation, string OpenAIKey)
        {
            var deleteClient = new RestClient("https://api.openai.com/v1/");

            //delete the vector store
            var deleteVectorRequest = new RestRequest("vector_stores/" + organisation.VectorStoreId, RestSharp.Method.Delete);
            deleteVectorRequest.AddHeader("Authorization", "Bearer " + OpenAIKey);
            deleteVectorRequest.AddHeader("Content-Type", "application/json");
            deleteVectorRequest.AddHeader("OpenAI-Beta", "assistants=v2");
            var deleteVectorResponse = deleteClient.Execute(deleteVectorRequest);
            organisation.VectorStoreId = null;

            var request = new RestRequest("assistants/" + Encryption.Decrypt(organisation.AIAssistantId), RestSharp.Method.Delete);
            request.AddHeader("Authorization", "Bearer " + OpenAIKey);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("OpenAI-Beta", "assistants=v1");
            var response = deleteClient.Execute(request);

            //Delete the documents
            var documentsList = organisation.AIDocuments;
            foreach (AIDocument document in documentsList)
            {
                if (!string.IsNullOrEmpty(document.FileId))
                {
                    var fileRequest = new RestRequest("files/" + document.FileId, RestSharp.Method.Delete);
                    fileRequest.AddHeader("Authorization", "Bearer " + OpenAIKey);
                    var fileResponse = deleteClient.Execute(fileRequest);
                    _organisationService.RemoveFile(organisation.Id, document);
                }
            }

            return "Assistant Deleted";
        }
        public string CreateAssistant(string AIAssistantId, string OpenAIKey, string AssistantName, string AIInstructions, List<string> fileIds, string vectorStoreId)
        {
            //create assistant
            try
            {
                var client = new RestClient("https://api.openai.com/v1/");
                RestRequest apirequest;
                if (string.IsNullOrEmpty(AIAssistantId))
                {
                    apirequest = new RestRequest("assistants", Method.Post);
                }
                else
                {
                    apirequest = new RestRequest("assistants/" + AIAssistantId, Method.Post);
                }
                apirequest.AddHeader("Authorization", "Bearer " + OpenAIKey);
                apirequest.AddHeader("Content-Type", "application/json");
                apirequest.AddHeader("OpenAI-Beta", "assistants=v2");
                apirequest.AddJsonBody(new
                {
                    instructions = AIInstructions,
                    name = AssistantName,
                    tools = new[]
                    {
                        new { type = "file_search" }
                    },
                    tool_resources = new
                    {
                        file_search = new
                        {
                            vector_store_ids = new[] { vectorStoreId }
                        }
                    },
                    model = "gpt-4o"

                });
                var assistantResponse = client.Execute(apirequest);
                var assistantResponseJson = JObject.Parse(assistantResponse.Content);
                return assistantResponseJson["id"].ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public ConversationMessageStarterResponse CreateConversation(Conversation Conversation, string OpenAIKey, AssistantRequest data, Organisation Organisation, string WebsiteURL, MessageService messageService, Message userMessage)
        {
            //code to call OpenAI API
            //need a thread if we are in a new conversation, the threadId to be stored on the conversation record
            var openApiClient = new RestClient("https://api.openai.com/v1/");
            if (string.IsNullOrEmpty(Conversation.ThreadId))
            {
                //create a new thread
                var threadRequest = new RestRequest("threads", Method.Post);
                threadRequest.AddHeader("Authorization", "Bearer " + OpenAIKey);
                threadRequest.AddHeader("OpenAI-Beta", "assistants=v1");
                var threadResponse = openApiClient.Execute(threadRequest);
                var threadResponseJson = JObject.Parse(threadResponse.Content);
                var threadId = threadResponseJson["id"].ToString();

                //update the conversation with the threadId
                Conversation.ThreadId = threadId;
                _conversationService.Update(Conversation);
            }

            //can now add the message to the thread
            if (data.IgnoreContent)
            {
                data.LastMessage = data.Messages;
            }
            var messageRequest = new RestRequest("threads/" + Conversation.ThreadId + "/messages", Method.Post);
            messageRequest.AddHeader("Authorization", "Bearer " + OpenAIKey);
            messageRequest.AddHeader("OpenAI-Beta", "assistants=v1");
            messageRequest.AddJsonBody(new
            {
                role = "user",
                content = data.LastMessage
            });
            var messageResponse = openApiClient.Execute(messageRequest);
            var messageResponseJson = JObject.Parse(messageResponse.Content);

            var MessageId = Guid.NewGuid();
            ConversationMessageStarterResponse conversationMessageStarterResponse = new ConversationMessageStarterResponse();
            conversationMessageStarterResponse.ThreadId = Conversation.ThreadId;
            conversationMessageStarterResponse.MessageId = MessageId.ToString();
            conversationMessageStarterResponse.ConversationId = Conversation.Id.ToString();
            conversationMessageStarterResponse.AssistantId = Encryption.Decrypt(Organisation.AIAssistantId);


            //run CreateOpenAIRun as a new task
            //Task.Run(() => CreateOpenAIRun(conversationMessageStarterResponse, OpenAIKey,Organisation,WebsiteURL,messageService));
            Task.Run(() =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var messageServiceFactory = scope.ServiceProvider.GetRequiredService<MessageService>();
                    var organisationServiceFactory = scope.ServiceProvider.GetRequiredService<OrganisationService>();
                    CreateOpenAIRun(conversationMessageStarterResponse, OpenAIKey, Organisation, WebsiteURL, messageServiceFactory, Conversation, organisationServiceFactory, userMessage).Wait();
                }
            });

            //need to return to the caller a payload that indicates thread is running, and the MessageId so they can poll the completion

            return conversationMessageStarterResponse;
        }

        //Create a run as a task, this will update the MessageStream. User can poll messageId to get the current MessageStream
        public async Task<ConversationCreateResponse> CreateOpenAIRun(ConversationMessageStarterResponse conversationMessageStarterResponse, string OpenAIKey, Organisation Organisation, string WebsiteURL, MessageService messageService, Conversation conversation, OrganisationService organisationService, Message userMessage)
        {
            HttpClient client = new HttpClient();
            var runRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/threads/" + conversationMessageStarterResponse.ThreadId + "/runs")
            {
                Content = new StringContent("{ \"assistant_id\": \"" + conversationMessageStarterResponse.AssistantId + "\", \"stream\": true }", System.Text.Encoding.UTF8, "application/json")
                //Content = new StringContent("{ \"assistant_id\": \"asst_qdRZDEWtaGYcI5eQ1q8nvb5v\", \"stream\": true }", System.Text.Encoding.UTF8, "application/json")
            };

            // Set headers
            runRequest.Headers.Add("Authorization", "Bearer " + OpenAIKey);
            //runRequest.Headers.Add("Content-Type", "application/json");
            runRequest.Headers.Add("OpenAI-Beta", "assistants=v2");

            var runId = "";
            int annotationCount = 1;
            using (var response = await client.SendAsync(runRequest, HttpCompletionOption.ResponseHeadersRead))
            {
                //response.EnsureSuccessStatusCode();

                // Stream the response
                using (var body = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(body))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();

                        if (!string.IsNullOrWhiteSpace(line) && runId == "")
                        {
                            //if the line inclunes id then lets get the value.
                            if (line.Contains("\"id\""))
                            {
                                string json = line.Substring(line.IndexOf('{'));

                                var id = JObject.Parse(json);
                                runId = id["id"].ToString();
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            //Console.WriteLine("Received: " + line);
                            //if its delta then ProcessLine
                            if (line.Contains("\"thread.message.delta\",\"delta\""))
                            {
                                var value = ProcessLine(line, conversationMessageStarterResponse); // Implement this method based on how you want to process each event
                                Message streamedMessage = messageService.GetMessage(Guid.Parse(conversationMessageStarterResponse.MessageId));
                                if (streamedMessage == null)
                                {
                                    //setup streamedMessage
                                    var newMessage = new Message();
                                    newMessage.Id = Guid.Parse(conversationMessageStarterResponse.MessageId);
                                    newMessage.Sender = "AI";
                                    newMessage.Created = DateTime.UtcNow;
                                    newMessage.Response = "";
                                    newMessage.StreamStatus = "Active";
                                    newMessage.MessageStream = "";
                                    newMessage.Tokens = 0;
                                    newMessage.Conversation = conversation;
                                    messageService.Create(newMessage);
                                }
                                streamedMessage = messageService.GetMessage(Guid.Parse(conversationMessageStarterResponse.MessageId));
                                streamedMessage.StreamStatus = "In Progress";
                                //streamedMessage.MessageStream = streamedMessage.MessageStream + value;
                                streamedMessage.MessageStream = Regex.Replace(streamedMessage.MessageStream + value, @"【[^】]*】", m => $"[{annotationCount++}]");
                                messageService.Update(streamedMessage);
                            }
                        }
                    }
                }

            }

            var openApiClient = new RestClient("https://api.openai.com/v1/");

            //even though we've streamed, now that the stream is finished we can get the final response as we always would.
            //run is complete so we can get the message responses
            var assistantRequest = new RestRequest("threads/" + conversationMessageStarterResponse.ThreadId + "/messages", Method.Get);
            assistantRequest.AddHeader("Authorization", "Bearer " + OpenAIKey);
            assistantRequest.AddHeader("OpenAI-Beta", "assistants=v1");
            var assistantResponse = openApiClient.Execute(assistantRequest);
            var assistantMessages = JsonConvert.DeserializeObject<OpenAIMessageResponse>(assistantResponse.Content);
            var assistantResponseMessages = assistantMessages.data
                .Where(m => m.role == "assistant")
            .OrderByDescending(m => DateTimeOffset.FromUnixTimeSeconds(m.created_at))
                .ToList();

            //get the usage from the run
            var usageRequest = new RestRequest("threads/" + conversationMessageStarterResponse.ThreadId + "/runs/" + runId, Method.Get);
            usageRequest.AddHeader("Authorization", "Bearer " + OpenAIKey);
            usageRequest.AddHeader("OpenAI-Beta", "assistants=v1");
            var usageResponse = openApiClient.Execute(usageRequest);
            var usageResponseJson = JObject.Parse(usageResponse.Content);

            ConversationCreateResponse conversationCreateResponse = new ConversationCreateResponse();
            conversationCreateResponse.RequestTokens = (int)usageResponseJson["usage"]["prompt_tokens"];
            conversationCreateResponse.ResponseTokens = (int)usageResponseJson["usage"]["completion_tokens"];

            string aIResponse = assistantResponseMessages.First().content[0].text.value;



            var finalMessageUpdate = messageService.GetMessage(Guid.Parse(conversationMessageStarterResponse.MessageId));

            var streamText = finalMessageUpdate.MessageStream;

            int referenceCount = 0;
            foreach (var annotation in assistantResponseMessages.First().content[0].text.annotations)
            {
                if (annotation != null)
                {
                    //add a blank line if we're on the first annotation
                    referenceCount=referenceCount+1;
                    if(referenceCount == 1)
                    {
                        streamText = streamText + "\n\n";
                    }
                    AIDocument document = organisationService.GetDocumentByFileId(Organisation.Id, annotation.file_citation.file_id);
                    var website = WebsiteURL;
                    var docLink = website + document.DocumentPath;
                    docLink = docLink.Replace(@"\", "/");
                    docLink = Uri.EscapeUriString(docLink);

                    streamText = streamText  + "[" + referenceCount + ". " + document.DocumentName + "](" + docLink + ")" + "  \n";
                }
            }

            conversationCreateResponse.FinalResponse = streamText;

            finalMessageUpdate.StreamStatus = "Complete";
            finalMessageUpdate.Response = streamText;
            finalMessageUpdate.MessageStream = streamText;
            finalMessageUpdate.Tokens = conversationCreateResponse.ResponseTokens;
            messageService.Update(finalMessageUpdate);

            var requestMessage = messageService.GetMessage(userMessage.Id);
            requestMessage.Tokens = conversationCreateResponse.RequestTokens;
            messageService.Update(requestMessage);


            return conversationCreateResponse;
        }
        string ProcessLine(string line, ConversationMessageStarterResponse conversationMessageStarterResponse)
        {

            string cleanJson = line.Substring(line.IndexOf('{'));

            JObject parsedJson = JObject.Parse(cleanJson);
            string value = parsedJson["delta"]["content"][0]["text"]["value"].ToString();

            return value;


        }
    }
}
