using Mango.Areas.Admin.ViewModels.Account;
using Mango.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVM adminLoginVM)
        {
            if (!ModelState.IsValid) return View();
            User admin = await _userManager.FindByNameAsync(adminLoginVM.Username);
            if (admin == null)
            {
                ModelState.AddModelError("", "Username or password is invalid");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(admin, adminLoginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is invalid");
                return View();
            }


            return RedirectToAction("index", "product");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("login", "account");
        }
    }
}
