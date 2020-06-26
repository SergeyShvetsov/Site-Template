using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
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
    }
}
