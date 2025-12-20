using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Extensions;
using urunsatisportali.Models;
using urunsatisportali.Models.ViewModels;
using urunsatisportali.Repositories;

namespace urunsatisportali.Controllers
{
    public class CartController : Controller
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly urunsatisportali.Data.ApplicationDbContext _context;

        public CartController(IGenericRepository<Product> productRepository, urunsatisportali.Data.ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart") ?? new CartViewModel();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1, string? returnUrl = null)
        {
            var productResult = _productRepository.GetById(productId);
            if (!productResult.IsSuccess || productResult.Data == null)
            {
                return NotFound();
            }
            var product = productResult.Data;

            // Load images for thumbnail
            _context.Entry(product).Collection(p => p.Images).Load();
            var imageUrl = product.Images?.FirstOrDefault()?.ImageUrl ?? "/assets/images/products/img-1.png";

            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart") ?? new CartViewModel();

            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            var currentQuantityInCart = existingItem?.Quantity ?? 0;

            if (currentQuantityInCart + quantity > product.StockQuantity)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = $"Yetersiz stok. Mevcut: {product.StockQuantity}" });
                }
                TempData["Error"] = "Yetersiz stok.";
                return RedirectToAction("Index"); // shop or detail? cart/index by default
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = imageUrl
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // If Ajax request, return JSON
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, cartCount = cart.Items.Sum(x => x.Quantity) });
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart");
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
                if (item != null)
                {
                    cart.Items.Remove(item);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart");
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    if (item.Quantity <= 0)
                        cart.Items.Remove(item);

                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }

        // Helper specifically for getting cart count via AJAX for header
        public IActionResult GetCartCount()
        {
            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart");
            int count = cart != null ? cart.Items.Sum(x => x.Quantity) : 0;
            return Json(count);
        }
    }
}
