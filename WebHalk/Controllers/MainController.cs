using Microsoft.AspNetCore.Mvc;
using WebHalk.Data;
using WebHalk.Models.Categories;
using WebHalk.Data.Entities;
using System.Linq;

namespace WebHalk.Controllers
{
    public class MainController : Controller
    {
        private readonly HulkDbContext _hulkDbcontext;

        public MainController(HulkDbContext hulkDbcontext)
        {
            _hulkDbcontext = hulkDbcontext;
        }

        public IActionResult Index()
        {
            var list = _hulkDbcontext.Categories
                .Select(x => new CategoryItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image
                })
                .ToList();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new CategoryEntity
                {
                    Name = model.Name,
                    Image = model.Image
                };
                _hulkDbcontext.Categories.Add(category);
                _hulkDbcontext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
