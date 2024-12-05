using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Stripe;
using Stripe.Checkout;

namespace MyEF2.WebApp.Pages
{
    public class PricingModel : PageModel
    {
        private readonly SettingService _settingService;
        private readonly StripeProductService _stripeProductService;
        private readonly UserService _userService;
        private readonly OrganisationService _organisationService;
        private readonly StripeSubscriptionService _stripeSubscriptionService;


        public PricingModel(SettingService settingService, StripeProductService stripeProductService,UserService userService,OrganisationService organisationService,StripeSubscriptionService stripeSubscriptionService)
        {
            _settingService = settingService;
            _stripeProductService = stripeProductService;
            _userService = userService;
            _organisationService = organisationService;
            _stripeSubscriptionService = stripeSubscriptionService;
        }
        public Setting Setting { get; set; }
        public List<StripeProduct> StripeProducts { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string? StripeProductId)
        {
            Setting = _settingService.GetSettings();
            if (Setting.UseFrontend == false)
            {
                return RedirectToPage("/Dashboard");
            }

            //if StripeProductId is not null, then we want to run the OnPostAsync method
            if(!string.IsNullOrEmpty(StripeProductId))
            {
                return await OnPostAsync(StripeProductId);
            }

            StripeProducts = _stripeProductService.GetStripeProducts().OrderBy(x=>x.Amount).Where(x=>x.HideFromPricing==false).ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string? StripeProductId)
        {
            //if StripeProductId is not in the paramater, it will be in the subscribe posted value as action
            if(string.IsNullOrEmpty(StripeProductId)){
            var action = Request.Form["subscribe"];
            
            //the value of action is the StripeProductId
            StripeProductId = action;
            }
            
            if (User.Identity.Name == null)
            {
                //create a paramater called stripeProductId and set it to the value of the form
                if(StripeProductId!=null){
                    return RedirectToPage("/Account/Register", new { StripeProductId = StripeProductId });
                }
                return RedirectToPage("/Account/Register");
            }

            
            

            Setting setting = _settingService.GetSettings();
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            var StripeProduct = _stripeProductService.GetStripeProduct(Guid.Parse(StripeProductId), thisUser.Email);
            string currentPageUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

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

            //create LineItems
            var lineItems = new List<SessionLineItemOptions>();

            //add subscription product to line items 
            lineItems.Add(new SessionLineItemOptions {
                Price = StripeProduct.PlanId,
                Quantity = 1,
            });

            //if there is a usage price, add it to the line items
            if(!string.IsNullOrEmpty(setting.StripeUsagePriceId)){
                SessionLineItemOptions usageLineItem = new SessionLineItemOptions
                {
                    Price = setting.StripeUsagePriceId,
                    

                };
                
                lineItems.Add(usageLineItem);
            }

            SessionSubscriptionDataOptions SubscriptionData = new SessionSubscriptionDataOptions();
            if(StripeProduct.TrialPeriodDays!=null && StripeProduct.TrialPeriodDays>0){
                SubscriptionData = new Stripe.Checkout.SessionSubscriptionDataOptions
                {
                    
                        TrialSettings = new Stripe.Checkout.SessionSubscriptionDataTrialSettingsOptions
                        {
                            EndBehavior = new Stripe.Checkout.SessionSubscriptionDataTrialSettingsEndBehaviorOptions
                            {
                                MissingPaymentMethod = "cancel",
                            },
                        },
                        TrialPeriodDays = StripeProduct.TrialPeriodDays,
                    
                };
            }

            var options = new SessionCreateOptions
            {
                // Specify the payment method types.

                LineItems = lineItems,
                
                Mode = "subscription",
                
                SuccessUrl = currentPageUrl + "/Administration/Stripe/Products/Success/" + stripeSubscription.Id,
                CancelUrl = currentPageUrl + "/Administration/Stripe/Products/Cancel",
                CustomerEmail=thisUser.Email,

                
                PaymentMethodCollection = "always",
                
            };

            if(SubscriptionData.TrialPeriodDays!=null)
            {
                options.SubscriptionData = SubscriptionData;
            }

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
