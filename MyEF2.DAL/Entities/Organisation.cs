using MyEF2.DAL.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
    public class Organisation
    {
        public Guid Id { get; set; }
        public string OrganisationName { get; set; }
        public string? ABN { get;set; }
        public bool EnforceMFA { get;set; }
        public bool EnableSSO { get;set; }
        public bool EnableAutoSSORegistration { get; set; }
        public string? EntityID { get; set; }
        public string? LoginURL { get; set; }
        public string? SigningCertificate { get; set; }
		public bool SkipEmailVerification { get; set; }
        public string? EmailDomainMask { get; set; }
        public string? SubscriptionPlan { get; set; } //SubscriptionPlan is the StripeProductPriceId
        public string? StripeSubscriptionId { get; set; } //StripeSubscriptionId is the Stripe Subscription ID

        public string? AssistantName { get; set; }
        public string? AIInstructions { get; set; }
        public List<AIDocument> AIDocuments { get; set; }
        public string? AIAssistantId { get; set; }
        public string? AIAssistantTargetWebsite { get; set; }
        public bool ShowAssistant { get; set; }
        public string? VectorStoreId { get; set; }

        public bool ActiveSubscription { get; set; }
        public string? StripeCustomerId { get; set; }
        // End Stadard Fields and Properties

        public string? AskAIAPI{ get; set; }
        public string? AskAIAPIKey { get; set; }
        public string? AskAIAssistantId { get; set; }
        
    }
}
