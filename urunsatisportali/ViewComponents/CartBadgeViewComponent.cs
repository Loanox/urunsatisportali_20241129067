using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Extensions;
using urunsatisportali.Models.ViewModels;

namespace urunsatisportali.ViewComponents
{
    public class CartBadgeViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObjectFromJson<CartViewModel>("Cart");
            var count = cart != null ? cart.Items.Sum(x => x.Quantity) : 0;
            return View(count);
        }
    }
}
