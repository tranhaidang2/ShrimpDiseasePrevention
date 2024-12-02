using Microsoft.AspNetCore.Mvc;
using ShrimpDiseasePrevention.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ShrimpDiseasePrevention.Controllers
{
    public class AccessController : Controller
    {
        private readonly ShrimpGuardContext _context;

        public AccessController(ShrimpGuardContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User _user)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.NotificationForRegister = "ModelInvalid";
                return View(_user);
            }

            try
            {
                if (await _context.Users.AnyAsync(u => u.UserName == _user.UserName))
                {
                    ViewBag.NotificationForRegister = "Already";
                    return View(_user);
                }

                if (_user.UserName.Length < 6)
                {
                    ViewBag.NotificationForRegister = "Username too short";
                    return View(_user);
                }

                if (_user.UserPassword.Length < 6)
                {
                    ViewBag.NotificationForRegister = "Userpassword too short";
                    return View(_user);
                }

                var user = new User
                {
                    UserName = _user.UserName,
                    UserPassword = BCrypt.Net.BCrypt.HashPassword(_user.UserPassword),
                    UserFullName = _user.UserFullName,
                    UserCreateAt = DateTime.UtcNow,
                    RoleId = 2
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                ViewBag.NotificationForRegister = "Success";
                return RedirectToAction("Login", "Access");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi xử lý yêu cầu. Vui lòng thử lại sau.");
                return View(_user);
            }
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string userPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))
                {
                    ViewBag.NotificationForLogin = "NULL";
                    return View();
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == userName);

                if (user != null && BCrypt.Net.BCrypt.Verify(userPassword, user.UserPassword))
                {
                    HttpContext.Session.SetString("FullName", user.UserFullName);
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserName", user.UserName);
                    HttpContext.Session.SetInt32("RoleId", user.RoleId);

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.NotificationForLogin = "Invalid";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.NotificationForLogin = "An error occurred while processing your request. Please try again.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
