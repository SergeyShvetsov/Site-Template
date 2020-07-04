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
    public class Cart : ViewComponent
    {
        private readonly ApplicationContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public Cart(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            db = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke()
        {
            int qty = 0;
            decimal price = 0;
            var model = new CartVM();

            // Проверитьсессию корзины
            if (_session.Keys.Contains("cart"))
            {
                // Получить общее количество товаров и цену
                var list = _session.Get<List<CartVM>>("cart");
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Total;
                }
                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                // или устанавливаем кол-во и цену в ноль
                model.Quantity = qty;
                model.Price = price;
            }

            return View(model);
        }
    }
}
