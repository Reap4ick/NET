using Microsoft.AspNetCore.Mvc;
using WebHalk.Data;
using WebHalk.Data.Entities;
using WebHalk.Models.Categories;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WebHalk.Controllers
{
    public class MainController : Controller
    {
        private readonly HulkDbContext _hulkDbContext;
        private readonly IWebHostEnvironment _env;

        public MainController(HulkDbContext hulkDbContext, IWebHostEnvironment env)
        {
            _hulkDbContext = hulkDbContext;
            _env = env;
        }

        public IActionResult Index()
        {
            var list = _hulkDbContext.Categories
                .Select(x => new CategoryItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image
                })
                .ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string imageUrl = string.Empty;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, model.ImageFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }
                imageUrl = $"/uploads/{model.ImageFile.FileName}";
            }
            else if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                imageUrl = model.ImageUrl;
            }

            CategoryEntity entity = new CategoryEntity()
            {
                Image = imageUrl,
                Name = model.Name,
            };
            _hulkDbContext.Categories.Add(entity);
            _hulkDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var entity = _hulkDbContext.Categories.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            var model = new CategoryEditViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageUrl = entity.Image
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = _hulkDbContext.Categories.Find(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.Name = model.Name;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, model.ImageFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }
                entity.Image = $"/uploads/{model.ImageFile.FileName}";
            }
            else if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                entity.Image = model.ImageUrl;
            }

            _hulkDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(CategoryDeleteViewModel model)
        {
            var entity = _hulkDbContext.Categories.Find(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            _hulkDbContext.Categories.Remove(entity);
            _hulkDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
