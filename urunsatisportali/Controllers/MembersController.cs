using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using urunsatisportali.Data;
using urunsatisportali.Models;

namespace urunsatisportali.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    public class MembersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MembersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var users = await _userManager.Users.ToListAsync();

            // Sort: Owner first, then others by creation date
            var sortedUsers = users.OrderByDescending(u => u.Id == currentUser?.Id && User.IsInRole("Owner"))
                                   .ThenByDescending(u => u.CreatedAt)
                                   .ToList();

            return View(sortedUsers);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            // Load Sales explicitly
            var sales = await _context.Sales
                .Where(s => s.UserId == id && !s.IsDeleted)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            ViewBag.Sales = sales;

            // Load user roles
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles = roles;

            return View(user);
        }

        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Load user roles
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = allRoles;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(string id, ApplicationUser model, string? newPassword, List<string>? selectedRoles)
        {
            if (id != model.Id) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                // Update user properties
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.PhoneNumber = model.PhoneNumber;
                user.IsAdmin = model.IsAdmin;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }

                // Update password if provided
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(user);
                    }
                }

                // Update roles
                if (selectedRoles != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (removeResult.Succeeded)
                    {
                        var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                        if (!addResult.Succeeded)
                        {
                            foreach (var error in addResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(user);
                        }
                    }
                }

                TempData["SuccessMessage"] = "Üye bilgileri başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            // Reload roles for view
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();
            ViewBag.UserRoles = userRoles;
            ViewBag.AllRoles = allRoles;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> ChangePassword(string userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(newPassword))
            {
                TempData["ErrorMessage"] = "Geçersiz istek.";
                return RedirectToAction(nameof(Details), new { id = userId });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Şifre değiştirme başarısız: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Details), new { id = userId });
        }
    }
}
