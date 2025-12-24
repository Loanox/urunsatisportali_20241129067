using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Repositories;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    public class ShopController : Controller
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly urunsatisportali.Data.ApplicationDbContext _context;

        public ShopController(IGenericRepository<Product> productRepository, IGenericRepository<Category> categoryRepository, urunsatisportali.Data.ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public IActionResult Index(int? categoryId, string? searchString, decimal? minPrice, decimal? maxPrice)
        {
            var categoryResult = _categoryRepository.GetAll();
            ViewBag.Categories = categoryResult.IsSuccess ? categoryResult.Data : new List<Category>();
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;

            var result = _productRepository.GetAll();
            if (!result.IsSuccess)
            {
                return View(new List<Product>());
            }

            var products = result.Data ?? new List<Product>();

            // Load primary images (simple load)
            foreach (var p in products)
            {
                _context.Entry(p).Collection(x => x.Images).Load();
            }

            var productQuery = products.AsQueryable();

            if (categoryId.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                productQuery = productQuery.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                                    || (p.Description != null && p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)));
            }

            if (minPrice.HasValue)
            {
                productQuery = productQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productQuery = productQuery.Where(p => p.Price <= maxPrice.Value);
            }

            return View(productQuery.ToList());
        }

        public IActionResult Details(int id)
        {
            var result = _productRepository.GetById(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            var product = result.Data;
            _context.Entry(product).Collection(p => p.Images).Load();

            // Fetch related products (Same Category, excluding current)
            if (product.CategoryId.HasValue)
            {
                var related = _context.Products
                   .Where(p => p.CategoryId == product.CategoryId && p.Id != id && !p.IsDeleted)
                   .Take(4)
                   .ToList();

                // Load images for related
                foreach (var rp in related)
                {
                    _context.Entry(rp).Collection(x => x.Images).Load();
                }

                ViewBag.RelatedProducts = related;
            }

            return View(product);
        }


    }
}
