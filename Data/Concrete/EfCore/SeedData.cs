using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EfCore
{
    public static class SeedData
    {
        public static byte[] LoadImageFromFile(string imagePath)
        {
            string path = "C:\\Users\\User\\Desktop\\Web-Gelistirme\\Asp-Net-New\\BlogApp\\BlogApp\\" + imagePath;
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                throw new FileNotFoundException("Dosya bulunamadı: " + path);
            }
        }


        public static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();
            if (context != null)
            {
                if (context.Database.GetPendingMigrations().Any())//Herhangi uygulanmamas migrations varsa
                {
                    context.Database.Migrate();
                }
                /* if (!context.Tags.Any())
                 {
                     context.Tags.AddRange(
                     new Tag { Text = "Web Programlama" });
                     context.SaveChanges();
                 }
                 if (!context.Users.Any())
                 {
                     context.Users.AddRange(
                     new User { UserName = "Yasin" });
                     context.SaveChanges();
                 }
                 if (!context.Posts.Any())
                 {
                     context.Posts.AddRange(
                     new Post
                     {
                         Title = "Asp.Net Core",
                         Content = "Asp.Net Core Dersleri",
                         IsActive = true,
                         PublishedOn = DateTime.Now.AddDays(-10),
                         Tags = context.Tags.Take(3).ToList(),
                         UserId = 1
                     });
                 }*/
                // Tag varlıklarını kontrol edip ekleme
                if (!context.Tags.Any())
                {
                    context.Tags.AddRange(
                        new Tag { Text = "Web Programlama", Url = "web-programlama", Color = TagColors.primary },
                        new Tag { Text = "C#", Url = "c#", Color = TagColors.secondary },
                        new Tag { Text = "ASP.NET Core", Url = "asp-net-core", Color = TagColors.success },
                        new Tag { Text = "Entity Framework Core", Url = "entity-framework-core", Color = TagColors.warning },
                        new Tag { Text = "LINQ", Url = "linq", Color = TagColors.danger },
                        new Tag { Text = "MVC", Url = "mvc", Color = TagColors.secondary },
                        new Tag { Text = "Mobile Development", Url = "mobile-development", Color = TagColors.danger },
                        new Tag { Text = "Xamarin", Url = "xamarin", Color = TagColors.danger },
                        new Tag { Text = "Flutter", Url = "flutter", Color = TagColors.primary }
                    );
                    context.SaveChanges();
                }

                // User varlıklarını kontrol edip ekleme
                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User { UserName = "Yasin", Image = LoadImageFromFile("\\wwwroot\\images\\user.png") },
                        new User { UserName = "Ayşe", Image = LoadImageFromFile("\\wwwroot\\images\\user.png") },
                        new User { UserName = "Ali", Image = LoadImageFromFile("\\wwwroot\\images\\user.png") },
                        new User { UserName = "Fatma", Image = LoadImageFromFile("\\wwwroot\\images\\user.png") },
                        new User { UserName = "Mehmet", Image = LoadImageFromFile("\\wwwroot\\images\\user.png") },
                        new User { UserName = "Elif", Image = LoadImageFromFile("\\wwwroot\\images\\user.png") }
                    );
                    context.SaveChanges();
                }

                // Post varlıklarını kontrol edip ekleme
                if (!context.Posts.Any())
                {
                    var users = context.Users.ToList();
                    var tags = context.Tags.ToList();
                    var webProgrammingTags = tags.Where(t => t.Text.Contains("Web") || t.Text.Contains("ASP.NET")).ToList();
                    var mobileDevelopmentTags = tags.Where(t => t.Text.Contains("Mobile") || t.Text.Contains("Xamarin") || t.Text.Contains("Flutter")).ToList();

                    var posts = new List<Post>
                {
                    new Post
                    {
                        Title = "Introduction to ASP.NET Core",
                        Content = "ASP.NET Core is a free and open-source web framework...",
                        IsActive = true,
                        PublishedOn = DateTime.Now.AddDays(-10),
                        Tags = webProgrammingTags,
                        UserId = users.First().UserId,
                       Image = LoadImageFromFile("\\wwwroot\\images\\asp.jpg")
                       ,Url="asp-net"
                    },
                    new Post
                    {
                        Title = "Building Mobile Apps with Xamarin",
                        Content = "Xamarin is a free and open source mobile UI framework...",
                        IsActive = true,
                        PublishedOn = DateTime.Now.AddDays(-20),
                        Tags = mobileDevelopmentTags,
                        UserId = users.Skip(1).First().UserId,
                       Image = LoadImageFromFile("\\wwwroot\\images\\xamarin.png"),
                       Url = "xamarin"
                    },
                    // Daha fazla Post ekleyebilirsiniz.
                };

                    context.Posts.AddRange(posts);
                    context.SaveChanges();
                }
            }
        }
    }
}