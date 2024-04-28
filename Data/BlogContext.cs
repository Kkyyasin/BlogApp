using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace BlogApp.Data.Concrete.EfCore
{
    public class BlogContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // IdentityUserRole<int> için birincil anahtarın tanımlanması
            modelBuilder.Entity<IdentityUserRole<int>>().HasKey(p => new { p.UserId, p.RoleId });

            // IdentityUserToken<int> için birincil anahtarın tanımlanması
            modelBuilder.Entity<IdentityUserToken<int>>().HasKey(p => new { p.UserId, p.LoginProvider, p.Name });

            // User tablosunun yapılandırılması
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // Veritabanında "Users" adında bir tablo kullanılacak

                // Alanların yapılandırılması
                entity.Property(u => u.Name).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Surname).IsRequired().HasMaxLength(50);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Opsiyonel alan yapılandırması
                entity.Property(u => u.Image).IsRequired(false);

                // İlişkilerin tanımlanması
                entity.HasMany(u => u.Posts).WithOne(p => p.User).HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Bir User silindiğinde, ilişkili tüm Posts da silinecek

                entity.HasMany(u => u.Comments).WithOne(c => c.User).HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // Bir User silindiğinde, ilişkili tüm Comments da silinecek
            });

            // Post tablosunun yapılandırılması
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Posts"); // Veritabanında "Posts" adında bir tablo kullanılacak

                // Birincil anahtar olarak PostId'nin tanımlanması
                entity.HasKey(p => p.PostId);

                // Alanların yapılandırılması
                entity.Property(p => p.Title).HasMaxLength(100);
                entity.Property(p => p.Url).HasMaxLength(255);
                entity.Property(p => p.Content);
                entity.Property(p => p.PublishedOn).HasDefaultValueSql("GETDATE()");
                entity.Property(p => p.IsActive).HasDefaultValue(true);

                // Opsiyonel alan yapılandırması
                entity.Property(p => p.Image).IsRequired(false);

                // User ile ilişkinin tanımlanması
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade); // User silindiğinde ilişkili Posts da silinecek

                // Tags ile olan çoktan çoğa ilişkinin tanımlanması
                entity.HasMany(p => p.Tags)
                    .WithMany(t => t.Posts);

                // Comments ile ilişkinin tanımlanması
                entity.HasMany(p => p.Comments)
                    .WithOne(c => c.Post)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade); // Post silindiğinde ilişkili Comments da silinecek
            });

            // Comment tablosunun yapılandırılması
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments"); // Veritabanında "Comments" adında bir tablo kullanılacak

                // Birincil anahtar olarak CommentId'nin tanımlanması
                entity.HasKey(c => c.CommentId);

                // Alanların yapılandırılması
                entity.Property(c => c.Text).IsRequired().HasMaxLength(500);
                entity.Property(c => c.PublishedOn).HasDefaultValueSql("GETDATE()");

                // Post ile ilişkinin tanımlanması
                entity.HasOne(c => c.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade); // Post silindiğinde ilişkili Comments da silinecek

                // User ile ilişkinin tanımlanması
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silindiğinde Comments silinmez
            });

            // Tag tablosunun yapılandırılması
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags"); // Veritabanında "Tag" adında bir tablo kullanılacak

                // Birincil anahtar olarak TagId'nin tanımlanması
                entity.HasKey(t => t.TagId);

                // Alanların yapılandırılması
                entity.Property(t => t.Text).HasMaxLength(100);
                entity.Property(t => t.Url).HasMaxLength(255);

                // Enum tipi için yapılandırma
                entity.Property(t => t.Color).HasConversion<string>();

                // Posts ile olan çoktan çoğa ilişkinin tanımlanması
                entity.HasMany(t => t.Posts)
                    .WithMany(p => p.Tags);
            });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=yasin\\SQLEXPRESS;Database=BlogApp;Trusted_Connection=True;Encrypt=False", options =>
                options.EnableRetryOnFailure())
            .LogTo(Console.WriteLine, LogLevel.Information);
        }

    }
}