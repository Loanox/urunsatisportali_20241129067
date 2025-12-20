using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IWebHostEnvironment _environment;

        public AuthController(SignInManager<ApplicationUser> signInManager,
                              UserManager<ApplicationUser> userManager,
                              ILogger<AuthController> logger,
                              IWebHostEnvironment environment)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Kullanıcı adı ve şifre gereklidir";
                return View();
            }

            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
                    return View();
                }

                // Check for Admin or Owner role
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                var isOwner = await _userManager.IsInRoleAsync(user, "Owner");

                if (!isAdmin && !isOwner)
                {
                    ViewBag.Error = "Bu panele erişim yetkiniz yok.";
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
                    return View();
                }

                user.LastLoginAt = DateTime.Now;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("Dashboard", "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                ViewBag.Error = "Giriş işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                if (_environment.IsDevelopment())
                {
                    ViewBag.Error += $" Hata: {ex.Message}";
                }
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

