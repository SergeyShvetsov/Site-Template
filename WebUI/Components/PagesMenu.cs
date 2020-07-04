using Data.Model;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebUI.Areas.Admin.Models;

namespace WebUI.Components
{
    public class PagesMenu : ViewComponent
    {
        private readonly ApplicationContext db;
        public PagesMenu(ApplicationContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var res = new StringBuilder();
            foreach(var item in db.Pages.OrderBy(o => o.Sorting).Where(w => w.Slug != "home").Select(x => new PageVM(x)))
            {
                res.Append($"<li class=\"nav-item\"><a class=\"nav-link\" href=\"/{item.Slug}\">{item.Title}</a></li>");
            }
            return new HtmlContentViewComponentResult(new HtmlString(res.ToString()));
        }
    }
}
