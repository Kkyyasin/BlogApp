using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BlogContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("database"));
});
builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
var app = builder.Build();

app.UseStaticFiles();
SeedData.TestVerileriniDoldur(app);
app.MapControllerRoute(
name: "post_detail",
pattern: "posts/{url}",
defaults: new { controller = "Post", action = "Detail" }
);
app.MapControllerRoute(
name: "post_by_tag",
pattern: "posts/tag/{tag}",
defaults: new { controller = "Post", action = "Index" }
);

app.MapControllerRoute(
name: "default",
pattern: "{controller=Post}/{action=Index}"
);


app.Run();
