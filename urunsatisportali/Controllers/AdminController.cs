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

                    return RedirectToAction(nameof(Confirmation), new { message = "Ürün başarıyla eklendi.", returnUrl = Url.Action(nameof(Products)) });
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

            var xreq2 = Request?.Headers["X-Requested-With"].ToString();
            if (string.Equals(xreq2, "XMLHttpRequest", StringComparison.Ordinal))
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
                    await _context.SaveChangesAsync();

                    var xreq3 = Request?.Headers["X-Requested-With"].ToString();
                    if (string.Equals(xreq3, "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        return Json(new { success = true, message = "Ürün başarıyla güncellendi." });
                    }
                    return RedirectToAction(nameof(Confirmation), new { message = "Ürün başarıyla güncellendi.", returnUrl = Url.Action(nameof(Products)) });
                }
                catch
                {
                    var xreq4 = Request?.Headers["X-Requested-With"].ToString();
                    if (string.Equals(xreq4, "XMLHttpRequest", StringComparison.Ordinal))
                    {
                        return Json(new { success = false, message = "Güncelleme sırasında bir hata oluştu." });
                    }
                    ModelState.AddModelError("", "Güncelleme sırasında bir hata oluştu.");
                    var categoryList = await _context.Categories.ToListAsync();
                    ViewBag.Categories = new SelectList(categoryList, "Id", "Name", product.CategoryId);
                    return View(product);
                }
            }
            else
            {
                _logger.LogWarning("Model validation failed for product {ProductId}. Errors: {Errors}",
                    id,
                    string.Join("; ", ModelState
                        .SelectMany(x => x.Value?.Errors.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>())));
            }

            var categoryListOuter = await _context.Categories.ToListAsync();
            ViewBag.Categories = new SelectList(categoryListOuter, "Id", "Name", product.CategoryId);

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
            return RedirectToAction(nameof(Confirmation), new { message = "Ürün başarıyla silindi.", returnUrl = Url.Action(nameof(Products)) });
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
        public async Task<IActionResult> CreateSale(Sale sale, [FromForm] List<int>? productIds, [FromForm] List<int>? quantities)
        {
            sale.SaleItems ??= new List<SaleItem>();
            var isAjax = string.Equals(Request?.Headers["X-Requested-With"].ToString(), "XMLHttpRequest", StringComparison.Ordinal);

            if (productIds == null || productIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Lütfen en az bir ürün seçin.");
            }

            if (productIds != null && (quantities == null || productIds.Count != quantities.Count))
            {
                ModelState.AddModelError(string.Empty, "Ürün kalemleri doğru biçimde gönderilmedi.");
            }

            decimal subtotal = 0m;
            if (productIds != null && quantities != null && productIds.Count > 0 && productIds.Count == quantities.Count)
            {
                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = await _context.Products.FindAsync(productIds[i]);
                    if (product == null)
                    {
                        ModelState.AddModelError(string.Empty, $"Seçilen ürün bulunamadı (ID: {productIds[i]}).");
                        continue;
                    }

                    var qty = quantities[i];
                    if (qty <= 0)
                    {
                        ModelState.AddModelError(string.Empty, $"{product.Name} için geçersiz adet girildi.");
                        continue;
                    }

                    if (product.StockQuantity < qty)
                    {
                        ModelState.AddModelError(string.Empty, $"{product.Name} için yeterli stok yok. (Mevcut: {product.StockQuantity})");
                        continue;
                    }

                    subtotal += product.Price * qty;
                }
            }

            var taxAmount = subtotal * (sale.Tax / 100m);
            var discountAmount = subtotal * (sale.Discount / 100m);
            sale.TotalAmount = subtotal;
            sale.FinalAmount = subtotal + taxAmount - discountAmount;

            if (sale.CustomerId <= 0 || !await _context.Customers.AnyAsync(c => c.Id == sale.CustomerId))
            {
                ModelState.AddModelError(nameof(sale.CustomerId), "Geçerli bir müşteri seçin.");
            }

            // Remove validation for server-side computed/assigned fields
            ModelState.Remove(nameof(Sale.SaleNumber));
            ModelState.Remove(nameof(Sale.TotalAmount));
            ModelState.Remove(nameof(Sale.FinalAmount));
            ModelState.Remove(nameof(Sale.Status));
            ModelState.Remove(nameof(Sale.SaleDate));
            ModelState.Remove(nameof(Sale.CreatedAt));

            if (isAjax && !ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new { field = x.Key, message = string.IsNullOrEmpty(e.ErrorMessage) ? "Bu alan geçersiz." : e.ErrorMessage }))
                    .ToList();
                var distinctMessages = errors.Select(e => e.message).Distinct().Take(10).ToList();
                var mainMessage = distinctMessages.Count > 0 ? "Lütfen şu hataları düzeltin: " + string.Join(", ", distinctMessages) : "Lütfen form hatalarını düzeltin.";
                return Json(new { success = false, message = mainMessage, errors });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Customers = await _context.Customers.ToListAsync();
                ViewBag.Products = await _context.Products.Where(p => p.IsActive && p.StockQuantity > 0).ToListAsync();
                return View(sale);
            }

            // Build sale and sale items
            sale.SaleNumber = $"SALE-{DateTime.Now:yyyyMMddHHmmss}";
            sale.SaleDate = DateTime.Now;
            sale.CreatedAt = DateTime.Now;
            sale.Status = "Completed";

            if (productIds != null && quantities != null)
            {
                for (int i = 0; i < productIds.Count; i++)
                {
                    var product = await _context.Products.FindAsync(productIds[i]);
                    if (product == null) continue;
                    var qty = quantities[i];
                    if (qty <= 0 || product.StockQuantity < qty) continue;

                    var saleItem = new SaleItem
                    {
                        ProductId = productIds[i],
                        Quantity = qty,
                        UnitPrice = product.Price,
                        TotalPrice = product.Price * qty,
                        CreatedAt = DateTime.Now
                    };
                    sale.SaleItems.Add(saleItem);
                    product.StockQuantity -= qty;
                    _context.Products.Update(product);
                }
            }

            if (!sale.SaleItems.Any())
            {
                var msg = "Geçerli bir satış kalemi yok. Lütfen ürün ve adet girin.";
                if (isAjax)
                {
                    return Json(new { success = false, message = msg, errors = new[] { new { field = "", message = msg } } });
                }
                ModelState.AddModelError(string.Empty, msg);
                ViewBag.Customers = await _context.Customers.ToListAsync();
                ViewBag.Products = await _context.Products.Where(p => p.IsActive && p.StockQuantity > 0).ToListAsync();
                return View(sale);
            }

            _context.Sales.Add(sale);
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("CreateSale saved changes: {Changes}", changes);

            if (isAjax)
            {
                return Json(new { success = true, message = "Satış başarıyla oluşturuldu.", redirectUrl = Url.Action(nameof(Sales)) });
            }
            return RedirectToAction(nameof(Confirmation), new { message = "Satış başarıyla oluşturuldu.", returnUrl = Url.Action(nameof(Sales)) });
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

