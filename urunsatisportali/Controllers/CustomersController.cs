using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Models;
using urunsatisportali.Repositories;

namespace urunsatisportali.Controllers
{
    [Authorize(Roles = "Admin, Owner")]
    public class CustomersController(IGenericRepository<Customer> repository, IGenericRepository<Sale> saleRepository) : Controller
    {
        private readonly IGenericRepository<Customer> _repository = repository;
        private readonly IGenericRepository<Sale> _saleRepository = saleRepository;

        // GET: Customers
        public IActionResult Index(string? searchString)
        {
            var result = _repository.GetAll();
            var customers = result.Data;

            if (!string.IsNullOrEmpty(searchString) && customers != null)
            {
                customers = customers.Where(s => s.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                                 (s.Email != null && s.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                                                 (s.Phone != null && s.Phone.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                                     .ToList();
            }

            return View(customers);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.Now;
                var result = _repository.Add(customer);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Message ?? "Kayıt sırasında bir hata oluştu.");
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public IActionResult Edit(int id)
        {
            var result = _repository.GetById(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }
            return View(result.Data);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Mevcut veriyi al (Created_At gibi alanları korumak için repo update mantığına güveniyoruz veya burada merge yapıyoruz)
                // GenericRepository Update metodu genellikle tüm entity'yi update eder.
                // İdealde DTO kullanılır ama burada entity kullanıyoruz.
                // Repositorymizdeki Update Attach mantığında çalışıyorsa sorun yok, ama manuel map gerekebilir.
                // GenericRepository implementation kontrol: _dbSet.Update(entity) yapıyor. Bu tüm alanları set eder.
                // Bu yüzden CreatedAt kaybolabilir eğer formdan gelmiyorsa.
                // Güvenli yöntem: DB'den çekip güncellemek.

                var existingResult = _repository.GetById(id);
                if (existingResult.IsSuccess && existingResult.Data != null)
                {
                    var existing = existingResult.Data;
                    existing.Name = customer.Name;
                    existing.Email = customer.Email;
                    existing.Phone = customer.Phone;
                    existing.Address = customer.Address;
                    existing.City = customer.City;
                    existing.PostalCode = customer.PostalCode;
                    existing.Country = customer.Country;
                    existing.UpdatedAt = DateTime.Now;

                    var updateResult = _repository.Update(existing);
                    if (updateResult.IsSuccess)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError(string.Empty, updateResult.Message ?? "Güncelleme hatası.");
                }
                else
                {
                    return NotFound();
                }
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Satış kontrolü
            var salesResult = _saleRepository.GetAll();
            if (salesResult.IsSuccess && salesResult.Data != null)
            {
                if (salesResult.Data.Any(s => s.CustomerId == id && !s.IsDeleted))
                {
                    TempData["ErrorMessage"] = "Bu müşteriye ait satış kayıtları bulunduğu için silinemez.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var result = _repository.Delete(id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
