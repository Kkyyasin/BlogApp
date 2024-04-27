using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BlogContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("database"));
});
builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();




var app = builder.Build();

app.UseStaticFiles();
SeedData.TestVerileriniDoldur(app);

app.UseRouting();

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
