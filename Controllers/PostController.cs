using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public PostController(IPostRepository postrepository, ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
            _postrepository = postrepository;
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
        public async Task<IActionResult> Detail2(int Id)
        {
            return View("Detail", await _postrepository.
            Posts.
            Include(p => p.Tags) //Tagslere erişim sağlar
            .Include(p => p.Comments) //Yorumlara erişim sağlar
            .ThenInclude(p => p.User).  //yorumu yapan kullaniciya erişim sağlar
            FirstOrDefaultAsync(p => p.PostId == Id));
        }
        public async Task<IActionResult> AddComments(int PostId, String UserName, String Text)
        {
            return RedirectToAction("Detail2", new { Id = PostId });
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
    }
}