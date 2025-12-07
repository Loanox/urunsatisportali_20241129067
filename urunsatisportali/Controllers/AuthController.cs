using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using urunsatisportali.Data;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IWebHostEnvironment _environment;

        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (IsLoggedIn())
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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
                    return View();
                }

                // Update last login
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();

                // Set cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.Now.AddDays(7)
                };

                Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);
                Response.Cookies.Append("Username", user.Username, cookieOptions);
                Response.Cookies.Append("IsAdmin", user.IsAdmin.ToString(), cookieOptions);

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
            if (IsLoggedIn())
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if username or email already exists
                    if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                    {
                        ViewBag.Error = "Bu kullanıcı adı zaten kullanılıyor";
                        return View(user);
                    }

                    if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    {
                        ViewBag.Error = "Bu e-posta adresi zaten kullanılıyor";
                        return View(user);
                    }

                    // Hash password
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    user.CreatedAt = DateTime.Now;
                    user.IsAdmin = false;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Auto login
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = Request.IsHttps,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddDays(7)
                    };

                    Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);
                    Response.Cookies.Append("Username", user.Username, cookieOptions);
                    Response.Cookies.Append("IsAdmin", user.IsAdmin.ToString(), cookieOptions);

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
                    return View(user);
                }
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("IsAdmin");
            return RedirectToAction("Login");
        }

        private bool IsLoggedIn()
        {
            return Request.Cookies.ContainsKey("UserId");
        }
    }
}

