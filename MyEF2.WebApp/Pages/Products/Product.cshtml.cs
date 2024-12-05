using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Product
{
    [Authorize]

    public class ProductModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly StatusService _statusService;
        private readonly UserService _userService;
        [BindProperty]
        public DAL.Models.NewProduct Product { get; set; }
        [BindProperty]
        public DAL.Entities.Product FullProduct { get; set; }
        [BindProperty]
        public List<DAL.Entities.Status> Statuses { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }

        public ProductModel(ProductService productService, StatusService statusService, UserService userService)
        {
            _productService = productService;
            _statusService = statusService;
            _userService = userService;
        }
        public void OnGet(string? id)
        {
            Statuses = _statusService.GetStatuses().OrderBy(s => s.StatusName).ToList();

            if (id != null)
            {
                //edit mode
                DAL.Entities.Product product = _productService.GetProduct(Guid.Parse(id),User.Identity.Name);

                FullProduct = product;

                Product = new NewProduct();

                Product.ProductName = product.ProductName;
                Product.Price = product.Price;
                Product.StatusId = product.Status.Id;

                FullProduct.CreatedDate= new MyTime(_userService).ConvertUTCToLocalTimeForUser(true,FullProduct.CreatedDate, User.Identity.Name);
                FullProduct.ModifiedDate = new MyTime(_userService).ConvertUTCToLocalTimeForUser(true, FullProduct.ModifiedDate, User.Identity.Name);

                
                
            }



        }
        public IActionResult OnPost(string? id)
        {
            var action = Request.Form["action"];

            if (string.Equals(action, "delete", StringComparison.OrdinalIgnoreCase))
            {
                //delete
                Guid guid = Guid.Parse(id);
                _productService.DeleteProduct(guid,User.Identity.Name);
                return RedirectToPage("Index");
            }

            ModelState.Remove("Status");
            ModelState.Remove("ProductName");
            ModelState.Remove("Organisation");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("ModifiedBy");
			if (ModelState.IsValid)
            {

                if (id == null)
                {
                    User user = _userService.GetUserByAuthId(User.Identity.Name);
                    Product.OrganisationId = user.Organisation.Id;
                    Product.CreatedById = user.Id;
                    Product.ModifiedById = user.Id;
                    _productService.CreateProduct(Product);
                }
                else
                {
					User user = _userService.GetUserByAuthId(User.Identity.Name);
                    Product.CreatedById = user.Id;
                    Product.ModifiedById = user.Id;
                    _productService.UpdateProduct(Guid.Parse(id), Product);
                }

                return RedirectToPage("Index");
            }

            return Page();
        }
    }         

}
