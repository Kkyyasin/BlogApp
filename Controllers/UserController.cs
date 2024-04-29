using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Data.Services.Interfaces;
using BlogApp.Entity;
using BlogApp.ExternalServices.Interfaces;
using BlogApp.Models;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlogApp.Controllers
{

    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _sıgnInManager;
        private readonly IUserService _userService;
        private readonly IEmailSender _emailsender;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public UserController(UserManager<User> userManager, IUserService userService, IEmailSender emailSender, SignInManager<User> signInManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _userService = userService;
            _emailsender = emailSender;
            _sıgnInManager = signInManager;
            _tokenGenerator = jwtTokenGenerator;
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
                var existingUser2 = await _userManager.FindByEmailAsync(model.Email);

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

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                    await _emailsender.SendEmailAsync(model.Email, "Hesabınızı Doğrulayın",
                        $"Lütfen hesabınızı doğrulamak için <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>buraya tıklayın</a>.");
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
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null) //Null kontrolu
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) //Kullanici kontrolu
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code); //email dogrulama
            if (result.Succeeded)
            {

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                return View("EmailConfirmed");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Error");
            }

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
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Giriş yapmadan önce e-posta adresinizi onaylamanız gerekmektedir.");
                    return View(model);
                }


                var result = await _sıgnInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {

                    var token = _tokenGenerator.GenerateJwtToken(user);
                    // Tokenı bir HTTP çerezi olarak ayarla
                    Response.Cookies.Append("JWTToken", token, new CookieOptions
                    {
                        HttpOnly = true, // Cookie, client-side scriptler tarafından erişilemez
                        Secure = true,   // Cookie yalnızca HTTPS üzerinden gönderilir
                        SameSite = SameSiteMode.Strict,// CSRF ataklarına karşı koruma sağlar
                        Expires = DateTimeOffset.UtcNow.AddHours(3)
                    });
                    // Kullanıcı için bir oturum cookie'si oluştur
                    await _sıgnInManager.SignInAsync(user, model.RememberMe);

                    return RedirectToAction("Index", "Post");
                }

                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
            }
            return View(model);
        }

    }
}
