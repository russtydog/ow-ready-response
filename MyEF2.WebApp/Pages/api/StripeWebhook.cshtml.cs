using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;
using Newtonsoft.Json;
using Stripe;

namespace MyEF2.WebApp.Pages.api
{
    [IgnoreAntiforgeryToken]
    public class StripeWebhookModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly UserService _userService;
        private readonly StripeProductService _stripeProductService;
        private readonly OrganisationService _organisationService;
        
        public StripeWebhookModel(SettingService settingService, UserService userService, StripeProductService stripeProductService,OrganisationService organisationService)
        {
            _settingService = settingService;
            _userService = userService;
            _stripeProductService = stripeProductService;
            _organisationService = organisationService;
        }
       
        
        [EnableCors("AllowAll")]
        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("StripeWebhookModel");
            
            var settings = _settingService.GetSettings();
            var _webhookSecret = settings.StripeWebhookId;
            
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _webhookSecret);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    Console.WriteLine("CheckoutSessionCompleted");
                    // Fulfill the purchase...
                    
                }
                if(stripeEvent.Type==Events.CustomerSubscriptionUpdated)
                {
                    Console.WriteLine("CustomerSubscriptionUpdated");
                    Console.WriteLine(json);
                    //json to StripeSubscriptionUpdateRequest
                    var subscriptionUpdateRequest = JsonConvert.DeserializeObject<StripeSubscriptionUpdateRequest>(json);
                    
                    var organisation = _organisationService.GetOrganisationByStripeCustomerId(subscriptionUpdateRequest.data.@object.customer);
                    if(organisation==null)
                    {
                        Console.WriteLine("Organisation not found");
                        HttpContext.Response.StatusCode = 200;
                        return new EmptyResult();
                    }
                    organisation.StripeSubscriptionId = Encryption.Encrypt(subscriptionUpdateRequest.data.@object.id);
                    var stripeProduct = _stripeProductService.GetStripeProduct(subscriptionUpdateRequest.data.@object.plan.id);
                    organisation.SubscriptionPlan=stripeProduct.Id.ToString();
                    organisation.ActiveSubscription=true;
                    _organisationService.UpdateOrganisations(organisation,"Webhook");

                }
                // ... handle the delete of subscription which happens at expiry date after cancallation
                if(stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    Console.WriteLine("CustomerSubscriptionDeleted");
                    Console.WriteLine(json);
                    var subscriptionUpdateRequest = JsonConvert.DeserializeObject<StripeSubscriptionUpdateRequest>(json);
                    
                    var organisation = _organisationService.GetOrganisationByStripeCustomerId(subscriptionUpdateRequest.data.@object.customer);
                    if(organisation==null)
                    {
                        Console.WriteLine("Organisation not found");
                        HttpContext.Response.StatusCode = 200;
                        return new EmptyResult();
                    }
                    organisation.StripeSubscriptionId = Encryption.Encrypt(subscriptionUpdateRequest.data.@object.id);
                    organisation.ActiveSubscription=false;
                    _organisationService.UpdateOrganisations(organisation,"Webhook");
                }
                

                HttpContext.Response.StatusCode = 200;
                return new EmptyResult(); // Return an empty result with the status code set
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.Message);
                HttpContext.Response.StatusCode = 400; // Bad Request
                return new EmptyResult();
    
            }
        }
    }
}
