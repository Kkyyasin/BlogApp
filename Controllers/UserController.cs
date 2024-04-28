using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Data.Services.Interfaces;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlogApp.Controllers
{

    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public UserController(UserManager<User> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                var existingUser2 = await _userManager.FindByEmailAsync(existingUser.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", $"Kullanıcı adı '{model.Username}' zaten kullanılmaktadır.");
                }

                if (existingUser2 != null)
                {
                    ModelState.AddModelError("", $"E-posta adresi '{model.Email}' zaten bir hesapla ilişkilendirilmiş.");
                }

                if (existingUser != null || existingUser2 != null)
                {
                    // Hata mesajlarını içeren ModelState ile formu geri dön
                    return View(model);
                }
                var result = await _userService.RegisterUserAsync(model);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    await _userService.EmailConfirmedAsync(user); //email dogrulama
                    return RedirectToAction("Login");
                }
                else
                {
                    // Şifre politikasına uymayan durumları işle
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }
            return View(model);
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Post");

            }
            return View();
        }
    }
}