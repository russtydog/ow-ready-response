using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using MyEF2.DAL.Models;
using RestSharp;
using System.Text.Json;


namespace MyEF2.WebApp.Pages.Administration.Invoicing
{
    [Authorize]
    public class IndexModel : PageModel
    {
		private readonly UserService _userService;
        private readonly OrganisationService _organisationService;
        private readonly SettingService _settingService;
		public IndexModel(UserService userService, OrganisationService organisationService, SettingService settingService)
		{
			_userService = userService;
            _organisationService = organisationService;
            _settingService = settingService;
		}
        public string InvoicesJson { get; set; }
        public string PendingInvoiceJson { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
			if (!thisUser.IsOrgAdmin)
			{
				return RedirectToPage("/Dashboard");
			}

            var settings  = _settingService.GetSettings();
			var organisation = _organisationService.GetOrganisation(thisUser.Organisation.Id);

            // Get Invoices from stripe
            var options = new RestClientOptions("https://api.stripe.com")
            {
            MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/v1/invoices?customer=" + organisation.StripeCustomerId, Method.Get);
            request.AddHeader("Authorization", "Bearer " + Encryption.Decrypt(settings.StripeSecretKey));

            RestResponse response = await client.ExecuteAsync(request);
            InvoicesJson = JsonSerializer.Serialize(response.Content);
            
            // Get Pending Invoices from stripe
            var request2 = new RestRequest("/v1/invoices/upcoming?customer=" + organisation.StripeCustomerId, Method.Get);
            request2.AddHeader("Authorization", "Bearer " + Encryption.Decrypt(settings.StripeSecretKey));

            RestResponse response2 = await client.ExecuteAsync(request2);
            PendingInvoiceJson = JsonSerializer.Serialize(response2.Content);
            Console.WriteLine(response2.Content);
			return Page();
		}
	}
}
