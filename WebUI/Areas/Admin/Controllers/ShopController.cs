using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
using Data.Model.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WebUI.Areas.Admin.Models;
using WebUI.Areas.Admin.Models.Shop;
using X.PagedList;

namespace WebUI.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        private ApplicationContext db;
        private IWebHostEnvironment _env;

        public ShopController(ApplicationContext cntx, IWebHostEnvironment env)
        {
            db = cntx;
            _env = env;
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

        // Урок 11
        [HttpGet]
        public IActionResult AddProduct()
        {
            var model = new ProductVM()
            {
                Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name"),
            };
            return View(model);
        }
        // Урок 12
        [HttpPost]
        public IActionResult AddProduct(ProductVM model, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");
                return View(model);
            }
            if (db.Products.Any(x => x.Name == model.Name))
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");
                ModelState.AddModelError("", "That product name is taken!");
                return View(model);
            }

            var product = new ProductDTO()
            {
                Name = model.Name,
                Slug = model.Name.Replace(" ", "-").ToLower(),
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId,
            };
            
            db.Add(product);
            db.SaveChanges();

            #region Upload Image
            // Создаем необходимые ссылки деректории
            var uplDir = Path.Combine(_env.WebRootPath, "Images\\Uploads");
            var pasString1 = Path.Combine(uplDir, "Products");
            var pasString2 = Path.Combine(uplDir, "Products\\" + product.Id);
            var pasString3 = Path.Combine(uplDir, "Products\\" + product.Id + "\\Thumbs");
            var pasString4 = Path.Combine(uplDir, "Products\\" + product.Id + "\\Gallery");
            var pasString5 = Path.Combine(uplDir, "Products\\" + product.Id + "\\Gallery\\Thumbs");

            var originalDirectory = new DirectoryInfo(uplDir);

            // Проверяем наличие директорий (если нет, то создаем)
            if (!Directory.Exists(pasString1)) Directory.CreateDirectory(pasString1);
            if (!Directory.Exists(pasString2)) Directory.CreateDirectory(pasString2);
            if (!Directory.Exists(pasString3)) Directory.CreateDirectory(pasString3);
            if (!Directory.Exists(pasString4)) Directory.CreateDirectory(pasString4);
            if (!Directory.Exists(pasString5)) Directory.CreateDirectory(pasString5);

            // Проверяем, был ли файл загружен
            if (file == null || file.Length == 0)
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");
                ModelState.AddModelError("", "The product image was not uploaded - wrong file lenght!");
                return View(model);
            }

            // Проверяем расширение файла
            var ext = file.ContentType.ToLower();
            if (ext != "image/jpg"
                && ext != "image/jpeg"
                && ext != "image/pjpeg"
                && ext != "image/gif"
                && ext != "image/x-png"
                && ext != "image/png"
                )
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");
                ModelState.AddModelError("", "The product image was not uploaded - wrong image format!");
                return View(model);
            }

            // объявляем переменную с именем изображения
            var imageName = file.FileName;

            // Сохраняем имя изображения в модель DTO
            product.ImageName = imageName;

            // Назначаем пути к оригинальному и уменьшенному изображению
            var path = $"{pasString2}\\{imageName}";
            var path2 = $"{pasString3}\\{imageName}";

            // Сохраняем оригинальное изображение
            using (var fileStream = new FileStream(path, FileMode.Create)) { file.CopyTo(fileStream); }

            // Создаем и сохраняем уменьшенное изображение
            var img = Image.Load(file.OpenReadStream());
            img.Mutate(x => x.Resize(200, 200)); ;
            img.Save(path2);

            db.SaveChanges();
            #endregion

            TempData["SM"] = "You have added a product!";
            // переадресовать пользователя
            return RedirectToAction(actionName: "AddProduct");
        }

        // Урок 13
        //[HttpPost]
        public IActionResult Products(int? page,Guid? catId)
        {
            // Устанавливаем номер страницы
            var pageNumber = page ?? 1;
            const int pageSize = 3;

            // Инициализируем List и заполняем данными
            List<ProductVM> listOfProductVM = db.Products
                .Where(w => catId == null || catId == Guid.Empty || w.CategoryId == (Guid)catId)
                .Select(s=> new ProductVM(s))
                .ToList();

            // Заполняем категории данными
            ViewBag.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");

            // Устанавливаем выбранную категорию
            ViewBag.SelectedCat = catId.ToString();

            // Устанавливаем постраничную навигацию
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, pageSize: pageSize);
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Возвращаем в преставление
            return View(listOfProductVM);
        }
    }
}
