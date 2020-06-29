using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
using Data.Model.Models;
using Microsoft.AspNetCore.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        private ApplicationContext db;
        public ShopController(ApplicationContext cntx)
        {
            db = cntx;
        }
        public IActionResult Categories()
        {
            var categories = db.Categories.OrderBy(o => o.Sorting)
                .Select(x => new CategoryVM(x))
                .ToList();

            return View(categories);
        }

        // Урок 9
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            if (db.Categories.Any(x => x.Name == catName)) return "titletaken";
            var dto = new CategoryDTO()
            {
                Name = catName,
                Slug = catName.Replace(" ", "-").ToLower(),
                Sorting = db.Categories.Any() ? db.Categories.Max(x => x.Sorting) + 1 : 1
            };
            db.Categories.Add(dto);
            db.SaveChanges();

            return dto.Id.ToString();

        }
        public IActionResult DeleteCategory(Guid id)
        {
            var item = db.Categories.Find(id);
            if (item != null)
            {
                db.Categories.Remove(item);
                db.SaveChanges();

                TempData["SM"] = "You have deleted a category!";
                return RedirectToAction("Categories");
            }

            return Content("The category does not exist.");
        }

        // Создаем метод сортировки
        [HttpPost]
        public void ReorderCategories(string ids)
        {
            var idsArray = ids.Replace("id_", "").Replace("[]=", "-").Split('&');
            int count = 1; // Реализуем счетчик  

            CategoryDTO dto;
            // Устанавливаем сортировку для каждой страницы
            foreach (var catId in idsArray)
            {
                var guid = new Guid(catId);
                dto = db.Categories.Find(guid);
                dto.Sorting = count;

                db.SaveChanges();
                count++;
            }
        }

        [HttpPost]
        public string RenameCategory(string newCatName, Guid id)
        {
            if (db.Categories.Any(x => x.Name == newCatName)) return "titletaken";
            var dto = db.Categories.Find(id);
            dto.Name = newCatName;
            dto.Slug = newCatName.Replace(" ", "-").ToLower();
            db.SaveChanges();

            return "ok";
        }
    }
}
