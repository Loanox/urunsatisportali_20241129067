using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Models;
using urunsatisportali.Repositories;

namespace urunsatisportali.Controllers
{
    [Authorize(Roles = "Admin, Owner")]
    public class CategoriesController(IGenericRepository<Category> repository) : Controller
    {
        private readonly IGenericRepository<Category> _repository = repository;

        public IActionResult Index()
        {
            var result = _repository.GetAll();
            return View(result.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var result = _repository.Add(category);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message ?? "Bir hata oluştu.");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var result = _repository.GetById(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = _repository.Update(category);
                if (result.IsSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message ?? "Bir hata oluştu.");
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
