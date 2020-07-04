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
using Data.Tools.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var pathString1 = Path.Combine(uplDir, "Products");
            var pathString2 = Path.Combine(uplDir, "Products\\" + product.Id);
            var pathString3 = Path.Combine(uplDir, "Products\\" + product.Id + "\\Thumbs");
            var pathString4 = Path.Combine(uplDir, "Products\\" + product.Id + "\\Gallery");
            var pathString5 = Path.Combine(uplDir, "Products\\" + product.Id + "\\Gallery\\Thumbs");

            var originalDirectory = new DirectoryInfo(uplDir);

            // Проверяем наличие директорий (если нет, то создаем)
            if (!Directory.Exists(pathString1)) Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2)) Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3)) Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4)) Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5)) Directory.CreateDirectory(pathString5);

            // Проверяем, был ли файл загружен
            if (file == null || file.Length == 0)
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");
                ModelState.AddModelError("", "The product image was not uploaded - wrong file lenght!");
                return View(model);
            }

            // Проверяем расширение файла
            if (!file.IsImage())
            {
                model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");
                ModelState.AddModelError("", "The product image was not uploaded - wrong image format!");
                return View(model);
            }

            // объявляем переменную с именем изображения
            var imageName = file.FileName;

            // Сохраняем имя изображения в модель DTO
            product.ImageName = imageName;
            db.SaveChanges();

            // Назначаем пути к оригинальному и уменьшенному изображению
            var path = $"{pathString2}\\{imageName}";
            var path2 = $"{pathString3}\\{imageName}";

            // Сохраняем оригинальное изображение
            using (var fileStream = new FileStream(path, FileMode.Create)) { file.CopyTo(fileStream); }

            // Создаем и сохраняем уменьшенное изображение
            var img = Image.Load(file.OpenReadStream());
            var newSize = img.ScaledImageSize(new Size(200, 200));
            img.Mutate(x => x.Resize(newSize));
            img.Save(path2);

            #endregion

            TempData["SM"] = "You have added a product!";
            // переадресовать пользователя
            return RedirectToAction(actionName: "AddProduct");
        }

        // Урок 13
        //[HttpPost]
        public IActionResult Products(int? page, Guid? catId)
        {
            // Устанавливаем номер страницы
            var pageNumber = page ?? 1;
            const int pageSize = 3;

            // Инициализируем List и заполняем данными
            List<ProductVM> listOfProductVM = db.Products
                .Where(w => catId == null || catId == Guid.Empty || w.CategoryId == (Guid)catId)
                .Select(s => new ProductVM(s))
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

        // Урок 14
        [HttpGet]
        public IActionResult EditProduct(Guid id)
        {
            var dto = db.Products.Find(id);
            if (dto == null)
            {
                return Content("That product does not exist.");
            }

            var model = new ProductVM(dto);
            model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");

            var uplDir = Path.Combine(_env.WebRootPath, "Images/Uploads/Products/" + id + "/Gallery/Thumbs");
            model.GalleryImages = Directory.EnumerateFiles(uplDir).Select(s => Path.GetFileName(s));

            return View(model);
        }
        [HttpPost]
        public IActionResult EditProduct(ProductVM model, IFormFile file)
        {
            // Получить Id продукта
            var id = model.Id;
            // Заполняем список категориями и изображениями
            model.Categories = new SelectList(db.Categories.ToList(), dataValueField: "Id", dataTextField: "Name");
            var uplDir = Path.Combine(_env.WebRootPath, "Images/Uploads/Products/" + id + "/Gallery/Thumbs");
            model.GalleryImages = Directory.EnumerateFiles(uplDir).Select(s => Path.GetFileName(s));

            // Проверяем модель на валидность
            if (!ModelState.IsValid) return View(model);

            // Проверяем имя продукта на уникальность
            if (db.Products.Any(a => a.Id != id && a.Name == model.Name))
            {
                ModelState.AddModelError("", "That product name is taken!");
                return View(model);
            }
            // Обновить продукт в БД
            var dto = db.Products.Find(id);
            dto.Name = model.Name;
            dto.Slug = model.Name.Replace(" ", "-").ToLower();
            dto.Description = model.Description;
            dto.Price = model.Price;
            dto.CategoryId = model.CategoryId;
            dto.ImageName = model.ImageName;
            db.SaveChanges();
            TempData["SM"] = "You have edited the product!";

            // Реализуем логику обработки изображений (Урок 15)
            #region Image Upload
            // Проверяем загрузку файла
            if (file != null && file.Length > 0)
            {
                if (!file.IsImage()) // Проверить расширение
                {
                    ModelState.AddModelError("", "The product image was not uploaded - wrong image format!");
                    return View(model);
                }

                // Установить пути загрузки
                var uploadDir = Path.Combine(_env.WebRootPath, "Images\\Uploads");
                var pathString1 = Path.Combine(uploadDir, "Products\\" + id);
                var pathString2 = Path.Combine(uploadDir, "Products\\" + id + "\\Thumbs");

                // Удаляем существующие файлы и директории
                var di1 = new DirectoryInfo(pathString1);
                var di2 = new DirectoryInfo(pathString2);
                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }
                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }
                // Сохранить имя изображение
                var imageName = file.FileName;
                dto.ImageName = imageName;
                db.SaveChanges();

                // Назначаем пути к оригинальному и уменьшенному изображению
                var path = $"{pathString1}\\{imageName}";
                var path2 = $"{pathString2}\\{imageName}";

                // Сохраняем оригинальное изображение
                using (var fileStream = new FileStream(path, FileMode.Create)) { file.CopyTo(fileStream); }

                // Создаем и сохраняем уменьшенное изображение
                var img = Image.Load(file.OpenReadStream());
                var newSize = img.ScaledImageSize(new Size(200, 200));
                img.Mutate(x => x.Resize(newSize)); ;
                img.Save(path2);
            }
            #endregion

            return RedirectToAction("EditProduct");
        }

        [HttpGet]
        public IActionResult DeleteProduct(Guid id)
        {
            // Удаляем товар из БД
            var dto = db.Products.Find(id);
            db.Products.Remove(dto);
            db.SaveChanges();

            // Удаляем дериктории товара (изображения)
            var uplDir = Path.Combine(_env.WebRootPath, "Images\\Uploads");
            var pathString = Path.Combine(uplDir, "Products\\" + id);

            var originalDirectory = new DirectoryInfo(uplDir);
            if (Directory.Exists(pathString)) Directory.Delete(pathString, true);

            TempData["SM"] = "The product was deleted!";
            return RedirectToAction("Products");
        }

        // Урок 16
        [HttpPost]
        public void SaveGalleryImages(Guid id)
        {
            // Перебрать все полученные файлы
            foreach (var file in Request.Form.Files)
            {
                // Проверить на null
                if (file != null && file.Length > 0)
                {
                    if (!file.IsImage()) continue;

                    // Назначить все пути к директориям
                    var uplDir = Path.Combine(_env.WebRootPath, "Images\\Uploads");
                    var pathString1 = Path.Combine(uplDir, "Products\\" + id + "\\Gallery");
                    var pathString2 = Path.Combine(pathString1, "Thumbs");

                    // Назначить пути изображений
                    var path = $"{pathString1}\\{file.FileName}";
                    var path2 = $"{pathString2}\\{file.FileName}";

                    // Сохраняем оригинальное изображение
                    using (var fileStream = new FileStream(path, FileMode.Create)) { file.CopyTo(fileStream); }

                    // Создаем и сохраняем уменьшенное изображение
                    var img = Image.Load(file.OpenReadStream());

                    var newSize = img.ScaledImageSize(new Size(200, 200));
                    img.Mutate(x => x.Resize(newSize));
                    img.Save(path2);
                }
            }
        }

        [HttpPost]
        public void DeleteImage(string id, string imageName)
        {
            var fulpath1 = Path.Combine(_env.WebRootPath, "Images/Uploads/Products/" + id + "/Gallery/" + imageName);
            var fulpath2 = Path.Combine(_env.WebRootPath, "Images/Uploads/Products/" + id + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fulpath1))
                System.IO.File.Delete(fulpath1);
            if (System.IO.File.Exists(fulpath2))
                System.IO.File.Delete(fulpath2);
        }

    }
}
