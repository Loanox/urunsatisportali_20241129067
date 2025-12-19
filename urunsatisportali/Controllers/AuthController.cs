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

        [HttpGet]
        public IActionResult SignUp()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(string username, string email, string password, string? fullName)
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
                    ViewBag.Error = "Bu kullanıcı adı zaten kullanılıyor";
                    return View();
                }

                var existingByEmail = await _userManager.FindByEmailAsync(email);
                if (existingByEmail != null)
                {
                    ViewBag.Error = "Bu e-posta adresi zaten kullanılıyor";
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

                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Dashboard", "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user sign up");
                ViewBag.Error = "Kayıt işlemi sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
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

