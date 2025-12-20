using urunsatisportali.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    public class AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 ILogger<AccountController> logger,
                                 ApplicationDbContext context) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<AccountController> _logger = logger;
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Kullanıcı adı ve şifre gereklidir.";
                return View();
            }

            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
                    return View();
                }

                // Optional: Prevent Admins from logging in via Customer page if desired,
                // but usually it's fine. However, they should be redirected appropriately.
                // For now, simple customer login.

                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
                    return View();
                }

                user.LastLoginAt = DateTime.Now;
                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during customer login");
                ViewBag.Error = "Giriş işlemi sırasında bir hata oluştu.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password, string? fullName)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Zorunlu alanlar eksik.";
                return View();
            }

            try
            {
                var existingByName = await _userManager.FindByNameAsync(username);
                if (existingByName != null)
                {
                    ViewBag.Error = "Bu kullanıcı adı zaten kullanılıyor.";
                    return View();
                }

                var existingByEmail = await _userManager.FindByEmailAsync(email);
                if (existingByEmail != null)
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
                    IsAdmin = false,
                    CreatedAt = DateTime.Now
                };

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    ViewBag.Error = string.Join(" ", createResult.Errors.Select(e => e.Description));
                    return View();
                }

                // Assign "User" role
                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during customer registration");
                ViewBag.Error = "Kayıt işlemi sırasında bir hata oluştu.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Clear any additional session data if needed
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var sales = await _context.Sales
                .Where(s => s.UserId == user.Id && !s.IsDeleted)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            ViewBag.Sales = sales;
            return View(user);
        }
    }
}
