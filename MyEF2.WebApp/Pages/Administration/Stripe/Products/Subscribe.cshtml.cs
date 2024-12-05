using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Stripe;
using Stripe.Checkout;

namespace MyEF2.WebApp.Pages.Administration.Stripe.Products
{
    [Authorize]
    public class SubscribeModel : PageModel
    {
        private readonly StripeProductService _stripeProductService;
        private readonly UserService _userService;
        private readonly SettingService _settingService;
        private readonly StripeSubscriptionService _stripeSubscriptionService;
        private readonly OrganisationService _organisationService;

        public SubscribeModel(StripeProductService stripeProductService, UserService userService, SettingService settingService, StripeSubscriptionService stripeSubscriptionService,OrganisationService organisationService)
        {
            _stripeProductService = stripeProductService;
            _userService = userService;
            _settingService = settingService;
            _stripeSubscriptionService = stripeSubscriptionService;
            _organisationService = organisationService;
        }
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        public StripeProduct StripeProduct { get; set; }
        
        public void OnGet(string? id)
        {
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsOrgAdmin)
            {
                RedirectToPage("/Index");
            }
            
            StripeProduct = _stripeProductService.GetStripeProduct(Guid.Parse(id), thisUser.Email);
        }
        public async Task<IActionResult> OnPostAsync(string? id)
        {
            Setting setting = _settingService.GetSettings();
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            StripeProduct = _stripeProductService.GetStripeProduct(Guid.Parse(id), thisUser.Email);
            string currentPageUrl = $"{Request.Scheme}://{Request.Host}";

            StripeConfiguration.ApiKey = Encryption.Decrypt(setting.StripeSecretKey);

            

            //if Organisation already has a subscription, then we want to update it.
            if(thisUser.Organisation.StripeSubscriptionId != null)
            {
                //get the subscription and update it.
                var subscriptionRetrieveService = new SubscriptionService();
                var subscription = subscriptionRetrieveService.Get(Encryption.Decrypt(thisUser.Organisation.StripeSubscriptionId));

                var oldPrice = StripeProduct.PlanId;
                var currentItemId = subscription.Items.Data[0].Id;

                var subscriptionupdateoptions = new SubscriptionUpdateOptions
                {
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Id = currentItemId,
                            Price = StripeProduct.PlanId,
                        },
                    },
                };
                var serviceSubscriptionUpdate = new SubscriptionService();
                var subscriptionServiceUpdate = serviceSubscriptionUpdate.Update(Encryption.Decrypt(thisUser.Organisation.StripeSubscriptionId), subscriptionupdateoptions);
                //it just updates so set the new price to the organisation.
                Organisation organisation = thisUser.Organisation;
                organisation.SubscriptionPlan = StripeProduct.Id.ToString();
                _organisationService.UpdateOrganisations(organisation,User.Identity.Name);
                return RedirectToPage("/Administration/Organisation");
            }

            StripeSubscription stripeSubscription = new StripeSubscription();
            stripeSubscription.Organisation = thisUser.Organisation;
            stripeSubscription.StripeSessionId = "";
            stripeSubscription.StripePlanId = StripeProduct.PlanId;
            stripeSubscription.StripeSubscriptionId = "";
            _stripeSubscriptionService.Create(stripeSubscription);

            var options = new SessionCreateOptions
            {
                // Specify the payment method types.

                LineItems = new List<SessionLineItemOptions> {
                new SessionLineItemOptions {
                  // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                  Price = StripeProduct.PlanId,
                  Quantity = 1,
                },
            },
                Mode = "subscription",
                
                

                SuccessUrl = currentPageUrl + "/Administration/Stripe/Products/Success/" + stripeSubscription.Id,
                CancelUrl = currentPageUrl + "/Administration/Stripe/Products/Cancel",
                CustomerEmail=thisUser.Email,
                
            };

            

            var service = new SessionService();
            Session session;
            try
            {
                session = service.Create(options);
                stripeSubscription.StripeSessionId = session.Id;
                stripeSubscription.StripeProductPriceId=StripeProduct.Id.ToString();
                _stripeSubscriptionService.Update(stripeSubscription);
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }
            catch (StripeException e)
            {
                Console.WriteLine($"StripeException caught: {e.Message}");
                return Page();
            }

            return Page();
        }
    }
}
