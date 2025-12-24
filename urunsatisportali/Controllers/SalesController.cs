using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Services;

namespace urunsatisportali.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    public class SalesController(ISaleService saleService) : Controller
    {
        private readonly ISaleService _saleService = saleService;

        public IActionResult Index()
        {
            var result = _saleService.GetAllSales();
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Message;
                return View(new List<urunsatisportali.Models.Sale>());
            }
            return View(result.Data);
        }

        public IActionResult Details(int id)
        {
            var result = _saleService.GetSaleById(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }
            return View(result.Data);
        }
    }
}
