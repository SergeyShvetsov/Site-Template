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
    public class Sidebar : ViewComponent
    {
        private readonly ApplicationContext db;
        public Sidebar(ApplicationContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = new SidebarVM(db.Sidebars.First());
            return new HtmlContentViewComponentResult(new HtmlString(model.Body));
        }
    }
}
