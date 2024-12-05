using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Services;
using Stripe;
using ProductService = Stripe.ProductService;


namespace MyEF2.WebApp.Pages.Administration.Stripe.Products
{
    
    [Authorize]
    public class ProductModel : PageModel
	{
		private readonly MyEF2.DAL.Services.StripeProductService _stripeProductService;
		private readonly MyEF2.DAL.Services.UserService _userService;
		private readonly MyEF2.DAL.Services.SettingService _settingService;

		public ProductModel(MyEF2.DAL.Services.StripeProductService stripeProductService, MyEF2.DAL.Services.UserService userService, MyEF2.DAL.Services.SettingService settingService)
		{
			_stripeProductService = stripeProductService;
			_userService = userService;
			_settingService = settingService;
		}

		[BindProperty]
		public StripeProduct StripeProduct { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }
        public async Task<IActionResult> OnGetAsync(string? id)
        {
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            if (!thisUser.IsAdmin)
            {
                return RedirectToPage("/Index");
            }

            if (id == null)
            {
				// Create
			}
			else
            {
				// Edit
				StripeProduct = _stripeProductService.GetStripeProduct(new Guid(id),thisUser.Email);
			}
            return Page();
        }
		public async Task<IActionResult> OnPostAsync(string? id)
		{
            User thisUser = _userService.GetUserByAuthId(User.Identity.Name);
            var action = Request.Form["action"];

            if (string.Equals(action, "delete", StringComparison.OrdinalIgnoreCase))
            {
                //delete
                _stripeProductService.Delete(Guid.Parse(id), thisUser.Email);
                return RedirectToPage("Index");
            }
            StripeProduct.Organisation=thisUser.Organisation;

            ModelState.Remove("StripeProduct.StripeProductPrices");
            ModelState.Remove("StripeProduct.Organisation");
            if (ModelState.IsValid)
			{

                Setting setting = _settingService.GetSettings();
				StripeConfiguration.ApiKey = Encryption.Decrypt(setting.StripeSecretKey);
				if(Id == null)
				{
                    var options = new ProductCreateOptions
					{
                        Name = StripeProduct.Name,
                        Description = StripeProduct.Description,
                        Active = true,
                        StatementDescriptor = StripeProduct.StatementDescriptor						
                    };
					var stripeService = new ProductService();
                    var stripeServiceResponse = stripeService.Create(options);
                    StripeProduct.StripeProductId = stripeServiceResponse.Id;

                    
                    var priceOptions = new PriceCreateOptions
                    {
                        Currency = "aud",
                        UnitAmount = Convert.ToInt32(StripeProduct.Amount*100),
                        Product = StripeProduct.StripeProductId,
                        Nickname = StripeProduct.Description,
                    };
                    if (StripeProduct.Frequency == "OnceOff")
                    {
                        priceOptions.BillingScheme = "per_unit";
                    }
                    else
                    {
                        priceOptions.Recurring = new PriceRecurringOptions { 
                            Interval = StripeProduct.Frequency, 
                            TrialPeriodDays=StripeProduct.TrialPeriodDays
                        };
                    }
                    var priceService = new PriceService();
                    var priceServiceResponse = priceService.Create(priceOptions);
                    StripeProduct.PlanId = priceServiceResponse.Id;
                    


                    StripeProduct.Organisation = thisUser.Organisation;
                    _stripeProductService.Create(StripeProduct,thisUser.Email);
                }
				else
				{
					StripeProduct.Id = Guid.Parse(id);

                    var options = new ProductUpdateOptions
                    {
                        Name = StripeProduct.Name,
                        Description = StripeProduct.Description,
                        Active = true,
                        StatementDescriptor = StripeProduct.StatementDescriptor
                    };
                    var stripeService = new ProductService();
                    var stripeServiceResponse = stripeService.Update(StripeProduct.StripeProductId, options);
                    StripeProduct.StripeProductId = stripeServiceResponse.Id;

                   
                    var priceUpdateOptions = new PriceUpdateOptions();
                    var priceOptions = new PriceCreateOptions();
                    if (StripeProduct.PlanId != "")
                    {
                        //it's an existing price however you can't update UnitAmount, so only if UnitAmount is different, then create a new price
                        var pricingService = new PriceService();
                        var existingPrice = pricingService.Get(StripeProduct.PlanId);
                        if (existingPrice.UnitAmount != Convert.ToInt32(StripeProduct.Amount * 100))
                        {
                            priceOptions.Currency = "aud";
                            priceOptions.UnitAmount = Convert.ToInt32(StripeProduct.Amount * 100);
                            priceOptions.Product = StripeProduct.StripeProductId;
                            priceOptions.Nickname = StripeProduct.Description;
                            if (StripeProduct.Frequency == "OnceOff")
                            {
                                priceOptions.BillingScheme = "per_unit";
                            }
                            else
                            {
                                priceOptions.Recurring = new PriceRecurringOptions { Interval = StripeProduct.Frequency };
                            }
                            var priceServiceResponse = pricingService.Create(priceOptions);

                            //set the original price to inactive
                            priceUpdateOptions.Active = false;
                            var priceService = new PriceService();
                            var priceInactiveServiceResponse = priceService.Update(StripeProduct.PlanId, priceUpdateOptions);

							StripeProduct.PlanId = priceServiceResponse.Id;
                        }
                        else
                        {
                            priceUpdateOptions.Nickname = StripeProduct.Description;
                            var priceService = new PriceService();
                            if (StripeProduct.PlanId != "")
                            {
                                var priceServiceResponse = priceService.Update(StripeProduct.PlanId, priceUpdateOptions);
                            }
                            else
                            {
                                var priceServiceResponse = priceService.Create(priceOptions);
								StripeProduct.PlanId = priceServiceResponse.Id;
                            }
                        }
                      
                    }
                    else
                    {

                        priceOptions.Currency = "aud";
                        priceOptions.UnitAmount = Convert.ToInt32(StripeProduct.Amount * 100);
                        priceOptions.Product = StripeProduct.StripeProductId;
                        priceOptions.Nickname = StripeProduct.Description;
                            
                        if (StripeProduct.Frequency == "OnceOff")
                        {
                            priceOptions.BillingScheme = "per_unit";
                        }
                        else
                        {
                            priceOptions.Recurring = new PriceRecurringOptions { Interval = StripeProduct.Frequency };
                        }
                        var priceService = new PriceService();
                        var priceServiceResponse = priceService.Create(priceOptions);
						StripeProduct.PlanId = priceServiceResponse.Id;
                    }


                    _stripeProductService.Update(StripeProduct, thisUser.Email);
				}



				return RedirectToPage("Index");
			}

            


            return Page();
		}
        
    }
}
