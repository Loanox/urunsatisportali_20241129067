using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<OwnerController> _logger;

        public OwnerController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               ILogger<OwnerController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(string username, string email, string password, string? fullName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Tüm alanlar zorunludur.";
                return View();
            }

            try
            {
                if (await _userManager.FindByNameAsync(username) != null)
                {
                    ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
                    return View();
                }

                if (await _userManager.FindByEmailAsync(email) != null)
                {
                    ViewBag.Error = "Bu e-posta adresi zaten kullanılıyor.";
                    return View();
                }

                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    IsAdmin = true, // Legacy flag
                    CreatedAt = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
                    return View();
                }

                // Assign Admin Role
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                await _userManager.AddToRoleAsync(user, "Admin");

                return RedirectToAction("Confirmation", "Admin", new { message = "Yeni yönetici başarıyla oluşturuldu." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin user");
                ViewBag.Error = "Bir hata oluştu: " + ex.Message;
                return View();
            }
        }
    }
}
