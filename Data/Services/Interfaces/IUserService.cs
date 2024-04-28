using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Data.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task EmailConfirmedAsync(User user);
    }
}