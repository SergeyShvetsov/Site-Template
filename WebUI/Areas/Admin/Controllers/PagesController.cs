using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
using Data.Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using WebUI.Areas.Admin.Models;
using Data.Tools.Extensions;

namespace WebUI.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        private ApplicationContext db;
        public PagesController(ApplicationContext cntx)
        {
            db = cntx;
        }
        public IActionResult Index()
        {
            // Объявляем список для представления {PagesVM}
            List<PageVM> pageList;
            // Инициализировать список 
            pageList = db.Pages.OrderBy(o => o.Sorting).Select(s => new PageVM(s)).ToList();
            // Возвращаем список в представление
            return View(pageList);
        }

        [HttpGet]
        public IActionResult AddPage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddPage(PageVM model)
        {
            // Выполнить валидацию
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Normalize();

            // Проверка уникальности заголовка и краткого описания
            if (db.Pages.Any(a => a.Title == model.Title))
            {
                ModelState.AddModelError("", "That title alredy exsist.");
                return View(model);
            }
            else if (db.Pages.Any(a => a.Slug == model.Slug))
            {
                ModelState.AddModelError("", "That slug alredy exsist.");
                return View(model);
            }

            db.Pages.Add(model.ToDbModel(null));
            db.SaveChanges();

            // Передаем сообщение через TempData
            TempData["SM"] = "You have added new page!";

            // Переадрессовываем пользователя на метод Index
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditPage(Guid id)
        {
            var item = db.Pages.Find(id);
            if (item != null)
            {
                return View(new PageVM(item));
            }

            return Content("The page does not exist.");
            //return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Normalize();

            // Проверка уникальности заголовка и краткого описания
            if (db.Pages.Any(a => a.Title == model.Title && a.Id != model.Id))
            {
                ModelState.AddModelError("", "That title alredy exsist.");
                return View(model);
            }
            else if (db.Pages.Any(a => a.Slug == model.Slug && a.Id != model.Id))
            {
                ModelState.AddModelError("", "That slug alredy exsist.");
                return View(model);
            }

            var dto = db.Pages.Find(model.Id);
            if (dto == null)
            {
                return Content("The page does not exist.");
            }
            model.ToDbModel(dto);
            db.SaveChanges();

            // Передаем сообщение через TempData
            TempData["SM"] = "You have edited page!";

            // Переадрессовываем пользователя на метод Index
            return RedirectToAction("EditPage");
        }

        public IActionResult PageDetails(Guid id)
        {
            var item = db.Pages.Find(id);
            if (item != null)
            {
                return View(new PageVM(item));
            }

            return Content("The page does not exist.");
        }

        public IActionResult DeletePage(Guid id)
        {
            var item = db.Pages.Find(id);
            if (item != null)
            {
                db.Pages.Remove(item);
                db.SaveChanges();

                TempData["SM"] = "You have deleted a page!";
                return RedirectToAction("Index");
            }


            return Content("The page does not exist.");
        }

        // Создаем метод сортировки
        [HttpPost]
        public void ReorderPages(string ids)
        {
            var idsArray = ids.Replace("id_", "").Replace("[]=", "-").Split('&');


            int count = 1; // Реализуем счетчик  

            PagesDTO dto;
            // Устанавливаем сортировку для каждой страницы
            foreach (var pageId in idsArray)
            {
                var guid = new Guid(pageId);
                dto = db.Pages.Find(guid);
                dto.Sorting = count;

                db.SaveChanges();
                count++;
            }
        }

        [HttpGet]
        public IActionResult EditSidebar()
        {
            var item = db.Sidebars.FirstOrDefault(); // TODO Изменить условия
            if (item != null)
            {
                return View(new SidebarVM(item));
            }

            return Content("The sidebar does not exist.");
            //return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EditSidebar(SidebarVM model)
        {
            var dto = db.Sidebars.Find(model.Id);
            model.ToDbModel(dto);
            db.SaveChanges();

            // Передаем сообщение через TempData
            TempData["SM"] = "You have edited sidebar!";
            // Переадрессовываем пользователя на метод Index
            return RedirectToAction("EditSidebar");
        }
    }
}
