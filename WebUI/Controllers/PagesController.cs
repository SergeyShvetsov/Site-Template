using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
using Data.Model.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WebUI.Areas.Admin.Models;

namespace WebUI.Controllers
{
    public class PagesController : Controller
    {
        private ApplicationContext db;
        private IWebHostEnvironment _env;

        public PagesController(ApplicationContext cntx, IWebHostEnvironment env)
        {
            db = cntx;
            _env = env;
        }

        // Урок 17
        public IActionResult Index(string page = "")
        {
            // Получить.установить краткий заголовок (Slug)
            if (page == "") page = "Home";

            // Объявляем модель и данные

            // Проверяем доступность страницы
            if (!db.Pages.Any(x => x.Slug.Equals(page))) return RedirectToAction("Index", new { page = "" });

            // Получаем контекст данных
            var dto = db.Pages.FirstOrDefault(x => x.Slug == page);

            // Устанавливаем заголовок страницы (Title)
            ViewBag.PageTitle = dto.Title;

            // Проверяем боковую панель
            ViewBag.SideBar = dto.HasSidebar;

            // Заполняем модель данными
            var model = new PageVM(dto);

            return View(model);
        }
    }
}
