using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using urunsatisportali.Models;
using urunsatisportali.Repositories;
using System.IO;

namespace urunsatisportali.Controllers
{
    [Authorize(Roles = "Admin, Owner")]
    public class ProductsController : Controller
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly urunsatisportali.Data.ApplicationDbContext _context;

        public ProductsController(IGenericRepository<Product> repository, IGenericRepository<Category> categoryRepository, IWebHostEnvironment hostEnvironment, urunsatisportali.Data.ApplicationDbContext context)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        // GET: Products
        public IActionResult Index(string? searchString, int? categoryId)
        {
            var result = _repository.GetAll();
            var products = result.Data;

            if (products != null)
            {
                // İlişkili verileri yükleme Generic Repository'de yoksa burada Lazy Loading veya explicit loading gerekebilir.
                // EF Core Lazy Loading açıksa sıkıntı yok. Değilse Category null gelir.
                // GenericRepository GetAll() .Include içermiyor. 
                // Bu durumda, GenericRepository'i geliştirmemiz veya bu Controller için özel repo kullanmamız gerekirdi.
                // Ancak "Paket 1" de Infra bitti denildi. Pratik çözüm: View'da null check veya 
                // Repository'ye Include desteği eklemek. Ama şimdilik basitçe filtrelemeyi yapalım.

                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                                   (p.SKU != null && p.SKU.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                                                   (p.Brand != null && p.Brand.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                                       .ToList();
                }

                if (categoryId.HasValue)
                {
                    products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
                }
            }

            var categories = _categoryRepository.GetAll().Data;
            ViewBag.Categories = categories;
            ViewBag.CategoriesSelectList = new SelectList(categories ?? [], "Id", "Name");

            return View(products);
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            var result = _repository.GetById(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            // Load images and category explicitly
            _context.Entry(result.Data).Collection(p => p.Images).Load();

            // Category bilgisini manuel çekelim eğer null ise (Generic Repo Include desteklemediği için)
            if (result.Data.Category == null && result.Data.CategoryId > 0)
            {
                var catResult = _categoryRepository.GetById(result.Data.CategoryId.Value);
                if (catResult.IsSuccess)
                    result.Data.Category = catResult.Data;
            }

            return View(result.Data);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var categories = _categoryRepository.GetAll().Data;
            ViewBag.Categories = new SelectList(categories ?? [], "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                // Image Validation
                if (images != null && images.Count > 4)
                {
                    ModelState.AddModelError("Images", "En fazla 4 fotoğraf yükleyebilirsiniz.");
                }

                if (images != null)
                {
                    foreach (var img in images)
                    {
                        if (img.Length > 7 * 1024 * 1024) // 7 MB
                        {
                            ModelState.AddModelError("Images", $"{img.FileName} boyutu 7 MB'dan büyük olamaz.");
                        }
                    }
                }

                // Category Validation
                if (product.CategoryId > 0)
                {
                    var catCheck = _categoryRepository.GetById(product.CategoryId.Value);
                    if (!catCheck.IsSuccess || catCheck.Data == null)
                    {
                        ModelState.AddModelError("CategoryId", "Seçilen kategori bulunamadı.");
                    }
                }

                if (ModelState.IsValid)
                {
                    product.CreatedAt = DateTime.Now;

                    // Handle Image Uploads
                    if (images != null && images.Count > 0)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string uploadDir = Path.Combine(wwwRootPath, "images", "products");
                        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                        foreach (var img in images)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                            string filePath = Path.Combine(uploadDir, fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await img.CopyToAsync(fileStream);
                            }

                            product.Images.Add(new ProductImage
                            {
                                ImageUrl = "/images/products/" + fileName
                            });
                        }
                    }

                    var result = _repository.Add(product);
                    if (result.IsSuccess)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(string.Empty, result.Message ?? "Hata oluştu.");
                }
            }

            var categories = _categoryRepository.GetAll().Data;
            ViewBag.Categories = new SelectList(categories ?? [], "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int id)
        {
            var result = _repository.GetById(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            // Load images explicit
            _context.Entry(result.Data).Collection(p => p.Images).Load();

            var categories = _categoryRepository.GetAll().Data;
            ViewBag.Categories = new SelectList(categories ?? [], "Id", "Name", result.Data.CategoryId);
            return View(result.Data);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, List<IFormFile> images, List<int> deleteImageIds)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingResult = _repository.GetById(id);
                if (existingResult.IsSuccess && existingResult.Data != null)
                {
                    var existing = existingResult.Data;
                    _context.Entry(existing).Collection(p => p.Images).Load();

                    // Calculate current image count
                    int currentImageCount = existing.Images.Count;
                    if (deleteImageIds != null)
                    {
                        currentImageCount -= deleteImageIds.Count;
                    }
                    int newImageCount = images != null ? images.Count : 0;

                    if (currentImageCount + newImageCount > 4)
                    {
                        ModelState.AddModelError("Images", $"En fazla 4 fotoğraf olabilir. Mevcut: {currentImageCount}, Yeni: {newImageCount}");
                    }

                    if (images != null)
                    {
                        foreach (var img in images)
                        {
                            if (img.Length > 7 * 1024 * 1024)
                            {
                                ModelState.AddModelError("Images", $"{img.FileName} boyutu 7 MB'dan büyük olamaz.");
                            }
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        // Delete requested images
                        if (deleteImageIds != null && deleteImageIds.Any())
                        {
                            var imagesToDelete = existing.Images.Where(i => deleteImageIds.Contains(i.Id)).ToList();
                            foreach (var img in imagesToDelete)
                            {
                                // Optional: Delete file from disk
                                var filePath = Path.Combine(_hostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/'));
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                existing.Images.Remove(img);
                            }
                        }

                        // Add new images
                        if (images != null && images.Count > 0)
                        {
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string uploadDir = Path.Combine(wwwRootPath, "images", "products");
                            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                            foreach (var img in images)
                            {
                                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                                string filePath = Path.Combine(uploadDir, fileName);

                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await img.CopyToAsync(fileStream);
                                }

                                existing.Images.Add(new ProductImage
                                {
                                    ImageUrl = "/images/products/" + fileName
                                });
                            }
                        }

                        existing.Name = product.Name;
                        existing.SKU = product.SKU;
                        existing.Description = product.Description;
                        existing.Price = product.Price;
                        existing.StockQuantity = product.StockQuantity;
                        existing.Brand = product.Brand;
                        existing.Unit = product.Unit;
                        existing.CategoryId = product.CategoryId;
                        existing.IsActive = product.IsActive;
                        existing.UpdatedAt = DateTime.Now;

                        var updateResult = _repository.Update(existing);
                        if (updateResult.IsSuccess)
                        {
                            return RedirectToAction(nameof(Index));
                        }
                        ModelState.AddModelError(string.Empty, updateResult.Message ?? "Güncelleme hatası.");
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            var categories = _categoryRepository.GetAll().Data;
            ViewBag.Categories = new SelectList(categories ?? [], "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _repository.Delete(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
