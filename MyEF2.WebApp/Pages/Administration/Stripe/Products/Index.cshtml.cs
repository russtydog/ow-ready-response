using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Administration.Stripe.Products
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly StripeProductService _stripeProductService;
        private readonly UserService _userService;
        public List<StripeProduct> StripeProducts { get; set; }
        public IndexModel(StripeProductService stripeProductService, UserService userService)
        {
			_stripeProductService = stripeProductService;
			_userService = userService;
		}
		
		public async Task<IActionResult> OnGetAsync()
        {
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsAdmin)
            {
                return RedirectToPage("/Index");
            }
            StripeProducts = _stripeProductService.GetStripeProducts().OrderBy(p => p.Name).ToList();
            return Page();
        }
    }
}
