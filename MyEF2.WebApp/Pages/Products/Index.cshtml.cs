using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyEF2.DAL.Entities;
using MyEF2.DAL.Models;
using MyEF2.DAL.Services;

namespace MyEF2.WebApp.Pages.Product
{
    [Route("/Products")]
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly UserService _userService;
        public List<DAL.Entities.Product> Products { get; set; }
        public DateTime LocalDate(DateTime utcDate)
        {

            return new MyTime(_userService).ConvertUTCToLocalTimeForUser(true, utcDate, User.Identity.Name);
        }

        public IndexModel(ProductService productService, UserService userService)
        {
            _productService = productService;
            _userService = userService;
        }
        public void OnGet()
        {
            User user = _userService.GetUserByAuthId(User.Identity.Name);
            Products = _productService.GetProducts(user.Organisation.Id).OrderBy(p => p.ProductName).ToList();

        }
    }
}
