using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEF2.DAL.Entities
{
	public class Setting
	{
		public Guid Id { get; set; }
		public string? ApplicationName { get; set; }
		public string? Logo { get; set; }
		public string? SMTPServer { get; set; }
		public string? SMTPUsername { get; set; }
		public string? SMTPPassword { get; set; }
		public int? SMTPPort { get; set; }
		public bool SMTPSSL { get; set; }
		public string? SMTPSenderName { get; set; }
		public string? SMTPSenderEmail { get; set; }
		public string? CompanyName { get; set; }
		public string FavIcon { get; set; }
		public bool UseOrganisations{get;set;}
		public bool EnableSSO { get;set;}
		public bool EnableAutoRegistration { get; set; }
		public string? EntityID { get; set; }
		public string? LoginURL { get; set; }
		public string? SigningCertificate { get; set; }
		public string? APIUrl { get; set; }
		public NotificationTemplate DefaultNotificationTemplate { get; set; }
		public string? CompanyWebsite { get; set; }
		public bool EnableRegistration { get; set; }
		public string? ActiveItemBackgroundColor { get; set; }
		public string? ActiveItemBackgroundColorHover { get; set; }
		public string? ItemTextColor { get; set; }
		public string? DefaultOrganisationId { get; set; }
		public bool EnableOrganisationSSO { get; set; }
		public string? StripeSecretKey { get; set; }

		public string? StripeWebhookId { get; set; }
		public string? StripeWebhookSecret { get; set; }
		public string? OpenAIKey { get; set; }
		public bool UseFrontend { get; set; }
		public string? TermsOfService { get; set; }
		public string? HeroTitle { get; set; }
        public string? HeroDescription{get;set;}
        public string? CustomersStatement{get;set;}
        public string? Feature1Title{get;set;}
        public string? Feature1Description{get;set;}
        public string? Feature1Highlight1Title{get;set;}
        public string? Feature1Highlight1Description{get;set;}
        public string? Feature1Hightlight2Title{get;set;}
        public string? Feature1Hightlight2Description{get;set;}
        public string? Feature2Title{get;set;}
        public string? Feature2Description{get;set;}
        public string? Feature2Highlight1Icon{get;set;}
        public string? Feature2Hightlight1Title{get;set;}
        public string? Feature2Hightlight1Description{get;set;}
        public string? Feature2Hightlight2Icon{get;set;}
        public string? Feature2Hightlight2Title{get;set;}
        public string? Feature2Hightlight2Description{get;set;}
        public string? FeaturesListTitle{get;set;}
        public string? FeaturesListFeature1Icon{get;set;}
        public string? FeaturesListFeature1Title{get;set;}
        public string? FeaturesListFeature1Description{get;set;}
        public string? FeaturesListFeature2Icon{get;set;}
        public string? FeaturesListFeature2Title{get;set;}
        public string? FeaturesListFeature2Description{get;set;}
        public string? FeaturesListFeature3Icon{get;set;}
        public string? FeaturesListFeature3Title{get;set;}
        public string? FeaturesListFeature3Description{get;set;}
        public string? FeaturesListFeature4Icon{get;set;}
        public string? FeaturesListFeature4Title{get;set;}
        public string? FeaturesListFeature4Description{get;set;}
        public string? FeaturesListFeature5Icon{get;set;}
        public string? FeaturesListFeature5Title{get;set;}
        public string? FeaturesListFeature5Description{get;set;}
        public string? FeaturesListFeature6Icon{get;set;}
        public string? FeaturesListFeature6Title{get;set;}
        public string? FeaturesListFeature6Description{get;set;}
        public string? Feature3Title{get;set;}
        public string? Feature3Description1{get;set;}
        public string? Feature3Description2{get;set;}
        public string? Accordian1Title{get;set;}
        public string? Accordian1Description{get;set;}
        public string? Accordian2Title{get;set;}
        public string? Accordian2Description{get;set;}
        public string? Accordian3Title{get;set;}
        public string? Accordian3Description{get;set;}
        public string? AccordianHeroTitle { get; set; }
        public string? AccordianHeroDescription1 { get; set; }
        public string? AccordianHeroDescription2 { get; set; }
        public bool EnableReviews{get;set;}
        public string? ReviewsTitle{get;set;}
        public int? Review1Rating{get;set;}
        public string? Review1Title{get;set;}
        public string? Review1Description{get;set;}
        public string? Review1Author{get;set;}
        public string? Review1AuthorCompany{get;set;}
        public int? Review2Rating{get;set;}
        public string? Review2Title{get;set;}
        public string? Review2Description{get;set;}
        public string? Review2Author{get;set;}
        public string? Review2AuthorCompany{get;set;}
        public int? Review3Rating{get;set;}
        public string? Review3Title{get;set;}
        public string? Review3Description{get;set;}
        public string? Review3Author{get;set;}
        public string? Review3AuthorCompany{get;set;}
        public int? Review4Rating{get;set;}
        public string? Review4Title{get;set;}
        public string? Review4Description{get;set;}
        public string? Review4Author{get;set;}
        public string? Review4AuthorCompany{get;set;}
        public string? DownloadActionTitle{get;set;}
        public string? DownloadActionDescription{get;set;}
        public string? SubscribeText{get;set;}
        public bool OrganisationsRequireSubscription { get; set; }
        public string? StripeCustomerPortalUrl { get; set; }
        public string? StripeUsagePriceId { get; set; }
        public string? StripeMeterEventName { get; set; }
        public string? SlickTrackerUrl { get; set; }
        // End Stadard Fields and Properties
		
}
}
