using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Data.Services.Interfaces;
using BlogApp.Entity;
using BlogApp.Models;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BlogApp.Data.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailsender;

        public UserService(UserManager<User> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailsender = emailSender;

        }
        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                PasswordHash = model.Password,
                Name = model.Name,
                Surname = model.Surname,
                Image = SeedData.LoadImageFromFile("\\wwwroot\\images\\user.png")
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }


    }
}