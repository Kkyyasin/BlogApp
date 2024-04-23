
using BlogApp.Data.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.ViewComponents
{
    public class TagsMenu : ViewComponent
    {

        private readonly ITagRepository _tagrepository;
        public TagsMenu(ITagRepository tagrepository)
        {
            _tagrepository = tagrepository;
        }
        public IViewComponentResult Invoke()
        {
            return View(_tagrepository.Tags.ToList());
        }

    }
}