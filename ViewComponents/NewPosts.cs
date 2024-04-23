using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.ViewComponents
{
    public class NewPosts : ViewComponent
    {
        private readonly IPostRepository _postrepository;
        public NewPosts(IPostRepository postrepository)
        {
            _postrepository = postrepository;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _postrepository.
            Posts
            .OrderByDescending(p => p.PublishedOn)  //Postları tarihe göre azalan şekilde sıralar
            .Take(5)    //5 tane alir
            .ToListAsync());
        }
    }
}