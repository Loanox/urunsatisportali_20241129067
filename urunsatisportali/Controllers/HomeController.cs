using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    public class HomeController(ILogger<HomeController> logger,
                                urunsatisportali.Repositories.IGenericRepository<Product> productRepository,
                                urunsatisportali.Data.ApplicationDbContext context,
                                urunsatisportali.Repositories.IGenericRepository<Category> categoryRepository) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly urunsatisportali.Repositories.IGenericRepository<Product> _productRepository = productRepository;
        private readonly urunsatisportali.Data.ApplicationDbContext _context = context;
        private readonly urunsatisportali.Repositories.IGenericRepository<Category> _categoryRepository = categoryRepository;

        public IActionResult Index()
        {
            var result = _productRepository.GetAll();
            var products = result.Data ?? new List<Product>();

            // Get Featured Categories
            var catResult = _categoryRepository.GetAll();
            if (catResult.IsSuccess && catResult.Data != null)
            {
                ViewBag.FeaturedCategories = catResult.Data.Take(4).ToList();
            }

            // Get latest 8 products
            var newArrivals = products.OrderByDescending(p => p.CreatedAt).Take(8).ToList();

            // Load images
            foreach (var p in newArrivals)
            {
                _context.Entry(p).Collection(x => x.Images).Load();
            }

            return View(newArrivals);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
