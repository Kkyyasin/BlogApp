using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            ViewBag.tags = _tagRepository.Tags.ToList();
            return View(_postrepository.Posts.ToList());
        }

    }
}