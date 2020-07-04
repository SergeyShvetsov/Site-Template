using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebUI.Areas.Admin.Models.Shop;

namespace WebUI.Controllers
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
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }
        // Урок 19
        public IActionResult Category(string id)
        {
            var catDTO = db.Categories.FirstOrDefault(x => x.Slug == id);
            if (catDTO == null)
            {
                // TODO Неорганизована проверка
            }

            var products = db.Products.Where(w => w.CategoryId == catDTO.Id).Select(s => new ProductVM(s)).ToList();
            //var products = new List<ProductVM>();
            //if (catDTO.Products != null)
            //    products = catDTO.Products.Select(s => new ProductVM(s)).ToList();

            ViewBag.CategoryName = catDTO.Name;
            return View(products);
        }

        [ActionName("product-details")]
        public IActionResult ProductDetails(string id)
        {

            var dto = db.Products.FirstOrDefault(x => x.Slug.Equals(id));
            if (dto == null)
            {
                return RedirectToAction("Index", controllerName: "Shop");
            }

            var model = new ProductVM(dto);
            // Получаем изображения из галереи
            var uplDir = Path.Combine(_env.WebRootPath, "Images/Uploads/Products/" + dto.Id + "/Gallery/Thumbs");
            model.GalleryImages = Directory.EnumerateFiles(uplDir).Select(s => Path.GetFileName(s));

            return View("ProductDetails", model);
        }
    }
}
