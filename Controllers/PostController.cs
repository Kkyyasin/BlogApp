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
        public PostController(IPostRepository postrepository)
        {
            _postrepository = postrepository;
        }
        public IActionResult Index()
        {
            return View(_postrepository.Posts.ToList());
        }

    }
}