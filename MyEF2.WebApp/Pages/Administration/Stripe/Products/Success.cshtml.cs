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
    public class SuccessModel : PageModel
    {
        private readonly StripeSubscriptionService _stripeSubscriptionService;
        private readonly OrganisationService _organisationService;
        
        public SuccessModel(StripeSubscriptionService stripeSubscriptionService,OrganisationService organisationService)
        {
            _stripeSubscriptionService = stripeSubscriptionService;
            _organisationService = organisationService;
        }
        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            //id is the StripeSubscriptionId, so get the checkout session from that
            StripeSubscription stripeSubscription = _stripeSubscriptionService.GetStripeSubscription(Guid.Parse(id));
                var service = new SessionService();
            var session = await service.GetAsync(stripeSubscription.StripeSessionId);

            if (session.PaymentStatus == "paid")
            {
                //update the subscription
                stripeSubscription.StripeSessionId = session.Id;
                stripeSubscription.StripeSubscriptionId = Encryption.Encrypt(session.SubscriptionId);
                _stripeSubscriptionService.Update(stripeSubscription);

                Organisation organisation = stripeSubscription.Organisation;
                organisation.SubscriptionPlan = stripeSubscription.StripeProductPriceId;
                organisation.StripeSubscriptionId = stripeSubscription.StripeSubscriptionId;
                organisation.StripeCustomerId=session.CustomerId;
                organisation.ActiveSubscription=true;
                _organisationService.UpdateOrganisations(organisation,User.Identity.Name);
                _stripeSubscriptionService.Delete(stripeSubscription);
                return RedirectToPage("/Administration/Organisation");
            }

            return Page();
        }
    }
}
