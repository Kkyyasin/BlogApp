using System.Text;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Data.Services.Implementations;
using BlogApp.Data.Services.Interfaces;
using BlogApp.Entity;
using BlogApp.ExternalServices;
using BlogApp.ExternalServices.Interfaces;
using BlogApp.Models;
using BlogApp.Services;
using BlogApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("database"));
});

//Repositories
builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

//Services
builder.Services.AddScoped<IUserService, UserService>();

//Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        // Şifre güvenlik ayarlarını yapılandırma
        options.Password.RequiredLength = 6; // Minimum şifre uzunluğu
        options.Password.RequireNonAlphanumeric = true; // En az bir alfasayısal olmayan karakter
        options.Password.RequireDigit = true; // En az bir rakam
        options.Password.RequireUppercase = true; // En az bir büyük harf
        options.Password.RequireLowercase = true; // En az bir küçük harf
    })
    .AddEntityFrameworkStores<BlogContext>()
    .AddDefaultTokenProviders().AddErrorDescriber<TurkishIdentityErrorDescriber>(); ;

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailSender, EmailService>(serviceProvider =>
{
    var emailSettings = serviceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
    return new EmailService(emailSettings.SmtpHost, emailSettings.SmtpPort, emailSettings.FromAddress, emailSettings.SmtpUsername, emailSettings.SmtpPassword);
});

// JWT ve Cookie tabanlı kimlik doğrulama için ayarlar
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/User/Login";
    options.LogoutPath = "/User/Logout";
    options.AccessDeniedPath = "/User/AccessDenied";
    options.Cookie.Name = "UserCookie";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddControllersWithViews();
var app = builder.Build();

app.UseStaticFiles();
SeedData.TestVerileriniDoldur(app);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{

    endpoints.MapControllerRoute(
        name: "post_detail",
        pattern: "posts/detail/{url}",
        defaults: new { controller = "Post", action = "Detail" }
    );

    // Tag'e göre postlar için güzergah
    endpoints.MapControllerRoute(
        name: "post_by_tag",
        pattern: "posts/tag/{tag}",
        defaults: new { controller = "Post", action = "Index" }
    );

    // Varsayılan güzergah
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Post}/{action=Index}/{id?}"
    );
});


app.Run();
