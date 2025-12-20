using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using urunsatisportali.Data;
using urunsatisportali.Models;
using Microsoft.Net.Http.Headers;

namespace urunsatisportali.Controllers
{
    [Authorize(Roles = "Admin, Owner")]
    public class AdminController(ApplicationDbContext context, ILogger<AdminController> logger) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<AdminController> _logger = logger;

        private bool IsAjax => Request.Headers.ContainsKey(HeaderNames.XRequestedWith) &&
                               string.Equals(Request.Headers[HeaderNames.XRequestedWith], "XMLHttpRequest", StringComparison.Ordinal);

        // Confirmation page
        [HttpGet]
        public IActionResult Confirmation(string message, string? returnUrl)
        {
            ViewBag.Message = string.IsNullOrWhiteSpace(message) ? "İşlem başarıyla tamamlandı." : message;
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? Url.Action(nameof(Dashboard)) : returnUrl;
            return View("~/Views/Shared/OperationConfirmation.cshtml");
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();
            var totalSales = await _context.Sales.CountAsync();
            var totalRevenue = await _context.Sales.SumAsync(s => s.FinalAmount);
            var lowStockProducts = await _context.Products.Where(p => p.StockQuantity < 10).CountAsync();
            var lowStockList = await _context.Products
                .Where(p => p.StockQuantity < 10)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
            var recentSales = await _context.Sales
                .Include(s => s.Customer)
                .OrderByDescending(s => s.SaleDate)
                .Take(10)
                .ToListAsync();

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalSales = totalSales;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.LowStockProducts = lowStockProducts;
            ViewBag.LowStockList = lowStockList;
            ViewBag.RecentSales = recentSales;

            return View();
        }










        // Helper methods

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

