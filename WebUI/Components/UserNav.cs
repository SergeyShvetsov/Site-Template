using Data.Model;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class UserNav : ViewComponent
    {
        private readonly ApplicationContext db;
        public UserNav(ApplicationContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke(bool isActive)
        {
            // получаем имя пользователя
            var userName = User.Identity.Name;
            // Получаем пользователя
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);
            var res = new StringBuilder();
            res.Append("<li class=\"navbar-text");
            if (isActive)
            {
                res.Append(" active");
            }
            res.Append($"\"><a href=\"/Account/UserProfile\" style=\"text-decoration:none;\">&nbsp;&nbsp;[{user.FirstName} {user.LastName}]</a></li>");
            return new HtmlContentViewComponentResult(new HtmlString(res.ToString()));
        }
    }
}
