using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogApp.Controllers
{

    public class PostController : Controller
    {
        private readonly IPostRepository _postrepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICommentRepository _commentrepository;

        public PostController(IPostRepository postrepository, ITagRepository tagRepository, ICommentRepository commentRepository)
        {
            _tagRepository = tagRepository;
            _postrepository = postrepository;
            _commentrepository = commentRepository;

        }

        public async Task<IActionResult> Index(string tag)
        {
            IQueryable<Post> posts = _postrepository.Posts.Include(p => p.Tags);//IQueryable daha veri tabanina gitmemiş
            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(p => p.Tags.Any(t => t.Url == tag));
            }

            // Filtrelenmiş ve include edilmiş sonuçlar veritabanından çekiliyor
            var resultPosts = await posts.ToListAsync();

            return View(resultPosts);
        }
        public async Task<IActionResult> Detail(string url)
        {
            return View(await _postrepository.
            Posts.
            Include(p => p.Tags) //Tagslere erişim sağlar
            .Include(p => p.Comments) //Yorumlara erişim sağlar
            .ThenInclude(p => p.User).  //yorumu yapan kullaniciya erişim sağlar
            FirstOrDefaultAsync(p => p.Url == url));
        }
        [HttpPost]
        public async Task<IActionResult> AddComments(int PostId, String UserName, String Text, String Url)
        {

            if (ModelState.IsValid)
            {
                Comment Entity = new Comment
                {
                    Text = Text,
                    PublishedOn = DateTime.Now,
                    PostId = PostId,
                    UserId = 1
                };
                _commentrepository.CreateComment(Entity);
                // İşlem başarılı mesajı
                return Json(new { success = true, message = "Yorum başarıyla eklendi!" });
            }
            // İşlem başarısız mesajı
            return Json(new { success = false, message = "Yorum eklenemedi, lütfen tekrar deneyin." });


        }
        public async Task<IActionResult> List(string? query)

        {
            IQueryable<Post> posts = _postrepository.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                posts = posts.Where(p => p.Title.Contains(query));

            }

            return View(await posts.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostStatus(int id, [FromBody] JsonElement body)
        {
            Console.WriteLine("TogglePostStatus called with postId: " + id);
            var isActive = body.GetProperty("isActive").GetBoolean();
            var post = await _postrepository.Posts.FirstOrDefaultAsync(post => post.PostId == id);
            if (post == null) return NotFound();

            post.IsActive = isActive;
            _postrepository.UpdatePost(post);

            return Json(new { success = true, message = "Status updated successfully." });
        }
    }
}
