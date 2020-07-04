using Data.Model;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Areas.Admin.Models;

namespace WebUI.Components
{
    public class CategoryMenu : ViewComponent
    {
        private readonly ApplicationContext db;
        public CategoryMenu(ApplicationContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = db.Categories.OrderBy(o=>o.Sorting).Select(s=> new CategoryVM(s));
            return View(model);
        }
    }
}
