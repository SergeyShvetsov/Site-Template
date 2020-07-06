using Data.Model;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Areas.Admin.Models;
using WebUI.Extensions;
using WebUI.Models.Cart;

namespace WebUI.Components
{
    public class PayPal : ViewComponent
    {
        public PayPal()
        {
        }

        public IViewComponentResult Invoke(IEnumerable<WebUI.Models.Cart.CartVM> model)
        {
            return View(model);
        }
    }
}
