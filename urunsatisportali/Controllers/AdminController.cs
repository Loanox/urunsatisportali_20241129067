using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using urunsatisportali.Data;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private bool IsLoggedIn()
        {
            return Request?.Cookies?.ContainsKey("UserId") == true;
        }

        private void CheckAuthentication()
        {
            if (!IsLoggedIn())
            {
                Response.Redirect("/Auth/Login");
            }
        }

        // Dashboard
        public async Task<IActionResult> Dashboard()
        {
            CheckAuthentication();
            var totalProducts = await _context.Products.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();
            var totalSales = await _context.Sales.CountAsync();
            var totalRevenue = await _context.Sales.SumAsync(s => s.FinalAmount);
            var lowStockProducts = await _context.Products.Where(p => p.StockQuantity < 10).CountAsync();
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
            ViewBag.RecentSales = recentSales;

            return View();
        }

        // Products Management
        public async Task<IActionResult> Products(string? searchString, int? categoryId)
        {
            var products = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) ||
                                               (p.SKU != null && p.SKU.Contains(searchString)) ||
                                               (p.Brand != null && p.Brand.Contains(searchString)));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories; // For dropdown in search
            ViewBag.CategoriesSelectList = new SelectList(categories, "Id", "Name"); // For product forms
            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            CheckAuthentication();
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (product.CategoryId > 0)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
                if (!categoryExists)
                {
                    ModelState.AddModelError("CategoryId", "Seçilen kategori bulunamadı.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.CreatedAt = DateTime.Now;
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    var xreq = Request?.Headers["X-Requested-With"].ToString();
                    if (string.Equals(xreq, "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        return Json(new { success = true, message = "Ürün başarıyla eklendi.", productId = product.Id });
                    }

                    return RedirectToAction(nameof(Products));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating product");
                    var xreq = Request?.Headers["X-Requested-With"].ToString();
                    if (string.Equals(xreq, "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        return Json(new { success = false, message = "Ürün eklenirken bir hata oluştu: " + ex.Message });
                    }
                    ModelState.AddModelError("", "Ürün eklenirken bir hata oluştu.");
                }
            }

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            {
                var xreq = Request?.Headers["X-Requested-With"].ToString();
                if (string.Equals(xreq, "XMLHttpRequest", StringComparison.Ordinal))
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e => new
                        {
                            field = x.Key,
                            message = string.IsNullOrEmpty(e.ErrorMessage) ? "Bu alan geçersiz." : e.ErrorMessage
                        }))
                        .ToList();

                    var errorMessages = errors.Select(e => e.message).Distinct().ToList();
                    var mainMessage = errorMessages.Count > 0
                        ? "Lütfen şu hataları düzeltin: " + string.Join(", ", errorMessages.Take(3))
                        : "Lütfen form hatalarını düzeltin.";

                    return Json(new { success = false, message = mainMessage, errors = errors });
                }
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            CheckAuthentication();
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product)
        {
            CheckAuthentication();

            if (id != product.Id)
            {
                var xreq = Request?.Headers["X-Requested-With"].ToString();
                if (string.Equals(xreq, "XMLHttpRequest", StringComparison.Ordinal))
                {
                    return Json(new { success = false, message = "Ürün ID'si eşleşmiyor." });
                }
                return NotFound();
            }

            if (product.CategoryId > 0)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
                if (!categoryExists)
                {
                    ModelState.AddModelError("CategoryId", "Seçilen kategori bulunamadı.");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    var errs = error.Value?.Errors;
                    if (errs != null)
                    {
                        foreach (var errorMessage in errs)
                        {
                            _logger.LogWarning("Validation error for {Field}: {Error}", error.Key, errorMessage.ErrorMessage);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        var xreq2 = Request?.Headers["X-Requested-With"].ToString();
                        if (string.Equals(xreq2, "XMLHttpRequest", StringComparison.Ordinal))
                        {
                            return Json(new { success = false, message = "Ürün bulunamadı." });
                        }
                        return NotFound();
                    }

                    existingProduct.Name = product.Name ?? existingProduct.Name;
                    existingProduct.SKU = product.SKU;
                    existingProduct.Description = product.Description;

                    if (product.CategoryId > 0)
                    {
                        existingProduct.CategoryId = product.CategoryId;
                    }

                    existingProduct.Price = product.Price;
                    existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.Brand = product.Brand;
                    existingProduct.Unit = product.Unit;
                    existingProduct.IsActive = product.IsActive;
                    existingProduct.UpdatedAt = DateTime.Now;

                    var changes = await _context.SaveChangesAsync();
                    _logger.LogInformation("Product {ProductId} updated successfully. Changes saved: {Changes}", id, changes);

                    var xreq3 = Request?.Headers["X-Requested-With"].ToString();
                    if (string.Equals(xreq3, "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        return Json(new { success = true, message = "Ürün başarıyla güncellendi." });
                    }

                    return RedirectToAction(nameof(Products));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Concurrency error updating product {ProductId}", id);
                    if (!ProductExists(product.Id))
                    {
                        var xreq4 = Request?.Headers["X-Requested-With"].ToString();
                        if (string.Equals(xreq4, "XMLHttpRequest", StringComparison.Ordinal))
                        {
                            return Json(new { success = false, message = "Ürün bulunamadı." });
                        }
                        return NotFound();
                    }
                    var errorMsg = "Ürün başka bir kullanıcı tarafından değiştirildi. Lütfen sayfayı yenileyip tekrar deneyin.";
                    var xreq5 = Request?.Headers["X-Requested-With"].ToString();
                    if (string.Equals(xreq5, "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    ModelState.AddModelError("", errorMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating product {ProductId}: {Message}", id, ex.Message);
                    var errorMsg = $"Ürün güncellenirken bir hata oluştu: {ex.Message}";
                    var xreq6 = Request?.Headers["X-Requested-With"].ToString();
                    if (string.Equals(xreq6, "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    ModelState.AddModelError("", errorMsg);
                }
            }
            else
            {
                _logger.LogWarning("Model validation failed for product {ProductId}. Errors: {Errors}",
                    id,
                    string.Join("; ", ModelState
                        .SelectMany(x => x.Value?.Errors.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>())));
            }

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            {
                var xreq = Request?.Headers["X-Requested-With"].ToString();
                if (string.Equals(xreq, "XMLHttpRequest", StringComparison.Ordinal))
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors.Select(e => new
                        {
                            field = x.Key,
                            message = string.IsNullOrEmpty(e.ErrorMessage) ? "Bu alan geçersiz." : e.ErrorMessage
                        }))
                        .ToList();

                    var errorMessages = errors.Select(e => e.message).Distinct().ToList();
                    var mainMessage = errorMessages.Count > 0
                        ? "Lütfen şu hataları düzeltin: " + string.Join(", ", errorMessages.Take(3))
                        : "Lütfen form hatalarını düzeltin.";

                    return Json(new { success = false, message = mainMessage, errors = errors });
                }
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Products));
        }

        // Categories Management
        public async Task<IActionResult> Categories()
        {
            return View(await _context.Categories.ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedAt = DateTime.Now;
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.UpdatedAt = DateTime.Now;
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Categories));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Categories));
        }

        // Customers Management
        public async Task<IActionResult> Customers(string? searchString)
        {
            var customers = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c => c.Name.Contains(searchString) ||
                                                (c.Email != null && c.Email.Contains(searchString)) ||
                                                (c.Phone != null && c.Phone.Contains(searchString)));
            }

            return View(await customers.ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.Now;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Customers));
            }

            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.UpdatedAt = DateTime.Now;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Customers));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Customers));
        }

        // Sales Management
        public async Task<IActionResult> Sales(string? searchString, string? status)
        {
            var sales = _context.Sales
                .Include(s => s.Customer)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                sales = sales.Where(s =>
                    s.SaleNumber.Contains(searchString) ||
                    (s.Customer != null && s.Customer.Name != null && s.Customer.Name.Contains(searchString)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                sales = sales.Where(s => s.Status == status);
            }

            return View(await sales.OrderByDescending(s => s.SaleDate).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> SaleDetails(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSale()
        {
            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsActive && p.StockQuantity > 0).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSale(Sale sale, List<int> productIds, List<int> quantities)
        {
            sale.SaleItems ??= new List<SaleItem>();

            if (productIds == null || productIds.Count == 0)
            {
                ModelState.AddModelError("", "Lütfen en az bir ürün seçin.");
            }

            if (productIds != null && (quantities == null || productIds.Count != quantities.Count))
            {
                ModelState.AddModelError("", "Ürün kalemleri doğru biçimde gönderilmedi.");
            }

            decimal subtotal = 0m;
            if (productIds != null && quantities != null && productIds.Count > 0 && productIds.Count == quantities.Count)
            {
                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = await _context.Products.FindAsync(productIds[i]);
                    if (product == null)
                    {
                        ModelState.AddModelError("", $"Seçilen ürün bulunamadı (ID: {productIds[i]}).");
                        continue;
                    }

                    if (product.StockQuantity < quantities[i])
                    {
                        ModelState.AddModelError("", $"{product.Name} için yeterli stok yok. (Mevcut: {product.StockQuantity})");
                    }

                    subtotal += product.Price * quantities[i];
                }
            }

            var taxAmount = subtotal * (sale.Tax / 100m);
            var discountAmount = subtotal * (sale.Discount / 100m);

            sale.TotalAmount = subtotal;
            sale.FinalAmount = subtotal + taxAmount - discountAmount;

            ModelState.Remove(nameof(sale.TotalAmount));
            ModelState.Remove(nameof(sale.FinalAmount));

            var isAjax = string.Equals(Request?.Headers["X-Requested-With"].ToString(), "XMLHttpRequest", StringComparison.Ordinal);

            if (ModelState.IsValid && productIds != null && productIds.Count > 0)
            {
                sale.SaleNumber = $"SALE-{DateTime.Now:yyyyMMddHHmmss}";
                sale.SaleDate = DateTime.Now;
                sale.CreatedAt = DateTime.Now;
                sale.Status = "Completed";

                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = await _context.Products.FindAsync(productIds[i]);
                    if (product == null || product.StockQuantity < quantities[i])
                    {
                        continue;
                    }
                    var saleItem = new SaleItem
                    {
                        ProductId = productIds[i],
                        Quantity = quantities[i],
                        UnitPrice = product.Price,
                        TotalPrice = product.Price * quantities[i],
                        CreatedAt = DateTime.Now
                    };
                    sale.SaleItems.Add(saleItem);

                    product.StockQuantity -= quantities[i];
                    _context.Products.Update(product);
                }

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                if (isAjax)
                {
                    return Json(new { success = true, redirectUrl = Url.Action(nameof(Sales)) });
                }

                return RedirectToAction(nameof(Sales));
            }

            if (isAjax)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new
                    {
                        field = x.Key,
                        message = string.IsNullOrEmpty(e.ErrorMessage) ? "Bu alan geçersiz." : e.ErrorMessage
                    }))
                    .ToList();

                var distinctMessages = errors.Select(e => e.message).Distinct().Take(10).ToList();
                var mainMessage = distinctMessages.Count > 0 ? "Lütfen şu hataları düzeltin: " + string.Join(", ", distinctMessages) : "Lütfen form hatalarını düzeltin.";

                return Json(new { success = false, message = mainMessage, errors });
            }

            ViewBag.Customers = await _context.Customers.ToListAsync();
            ViewBag.Products = await _context.Products.Where(p => p.IsActive && p.StockQuantity > 0).ToListAsync();
            return View(sale);
        }

        // Helper methods
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

