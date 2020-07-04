using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using WebUI.Extensions;
using WebUI.Models.Cart;

namespace WebUI.Controllers
{
    public class CartController : Controller
    {
        private ApplicationContext db;
        private IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public CartController(ApplicationContext cntx, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            db = cntx;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }
        // Урок 20
        public IActionResult Details()
        {
            var cart = _session.Get<List<CartVM>>("cart") ?? new List<CartVM>();
            if (!cart.Any())
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }

            ViewBag.GrandTotal = cart.Sum(s => s.Total);
            return View(cart);
        }

        // Урок 21
        public IActionResult AddToCartPartial(string id)
        {
            var uid = new Guid(id);
            var cart = _session.Get<List<CartVM>>("cart") ?? new List<CartVM>();
            var model = new CartVM();

            // Получаем продуст
            var product = db.Products.Find(uid);
            var productInCart = cart.FirstOrDefault(x => x.Id == uid);
            if (productInCart == null)
            {
                cart.Add(new CartVM
                {
                    Id = product.Id,
                    Name = product.Name,
                    Quantity = 1,
                    Price = product.Price,
                    Image = product.ImageName
                });
            }
            else
            {
                productInCart.Quantity++;
            }
            int qty = 0;
            decimal price = 0m;
            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Total;
            }
            model.Quantity = qty;
            model.Price = price;
            // Сохранить состояние в сессию
            _session.Set<List<CartVM>>("cart", cart);

            return PartialView("_AddToCartPartial", model);
        }

        [HttpGet]
        public JsonResult IncrementProduct(string id)
        {
            var uid = new Guid(id);
            var cart = _session.Get<List<CartVM>>("cart");
            var model = cart.FirstOrDefault(x => x.Id == uid);
            model.Quantity++;

            _session.Set<List<CartVM>>("cart", cart);

            // Возвращаем JSON ответ с данными
            return Json(new { qty = model.Quantity, price = model.Price }, new System.Text.Json.JsonSerializerOptions());
        }
        // Урок 22
        [HttpGet]
        public JsonResult DecrementProduct(string id)
        {
            var uid = new Guid(id);
            var cart = _session.Get<List<CartVM>>("cart");
            var model = cart.FirstOrDefault(x => x.Id == uid);
            if (model.Quantity > 1)
            {
                model.Quantity--;
            }
            else
            {
                model.Quantity = 0;
                cart.Remove(model);
            }

            _session.Set<List<CartVM>>("cart", cart);

            // Возвращаем JSON ответ с данными
            return Json(new { qty = model.Quantity, price = model.Price }, new System.Text.Json.JsonSerializerOptions());
        }
        [HttpGet]
        public void RemoveProduct(string id)
        {
            var uid = new Guid(id);
            var cart = _session.Get<List<CartVM>>("cart");
            var model = cart.FirstOrDefault(x => x.Id == uid);
            cart.Remove(model);
            _session.Set<List<CartVM>>("cart", cart);
        }
    }
}
