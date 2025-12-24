using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Extensions;
using urunsatisportali.Models;
using urunsatisportali.Models.ViewModels;
using urunsatisportali.Services;

namespace urunsatisportali.Controllers
{
    public class CheckoutController(ISaleService saleService, urunsatisportali.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly ISaleService _saleService = saleService;
        private readonly urunsatisportali.Data.ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public IActionResult Index()
        {
            // Check if user is authenticated
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                TempData["CheckoutMessage"] = "Satın almak için üye olmanız gerekmektedir. Lütfen giriş yapın veya kayıt olun.";
                return RedirectToAction("Login", "Account", new { returnUrl = "/Checkout" });
            }

            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart");
            if (cart == null || cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel { Cart = cart };
            return View(model);
        }

        [HttpPost]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart");
            if (cart == null || cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Shop");
            }

            model.Cart = cart; // Ensure cart is available for re-display

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var sale = new Sale
            {
                TotalAmount = cart.GrandTotal,
                Discount = 0,
                Tax = 0,
                FinalAmount = cart.GrandTotal,
                Notes = $"Name: {model.FirstName} {model.LastName}, Address: {model.Address}, {model.City}/{model.Country}, Email: {model.Email}",
                Status = "Completed"
            };

            // Link to User if logged in
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                sale.UserId = userId;
            }

            var productIds = cart.Items.Select(x => x.ProductId).ToList();
            var quantities = cart.Items.Select(x => x.Quantity).ToList();

            var result = _saleService.CreateSale(sale, productIds, quantities);

            if (result.IsSuccess && result.Data != null)
            {
                // Clear Cart
                HttpContext.Session.Remove("Cart");
                return RedirectToAction("OrderConfirmation", new { id = result.Data.Id });
            }
            else
            {
                ModelState.AddModelError("", result.Message ?? "Bir hata oluştu.");
                return View("Index", model);
            }
        }

        public IActionResult OrderConfirmation(int id)
        {
            var result = _saleService.GetSaleById(id);
            if (!result.IsSuccess) return NotFound();
            return View(result.Data);
        }
    }
}
