using System;
using System.Collections.Generic;
using System.Linq;
using Data.Model;
using Data.Model.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var productInCart = cart.FirstOrDefault(x => x.ProductId == uid);
            if (productInCart == null)
            {
                cart.Add(new CartVM
                {
                    ProductId = product.Id,
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
            var model = cart.FirstOrDefault(x => x.ProductId == uid);
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
            var model = cart.FirstOrDefault(x => x.ProductId == uid);
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
            var model = cart.FirstOrDefault(x => x.ProductId == uid);
            cart.Remove(model);
            _session.Set<List<CartVM>>("cart", cart);
        }

        // Урок 26
        public IActionResult PayPalPartial()
        {
            var cart = _session.Get<List<CartVM>>("cart") ?? new List<CartVM>();
            return PartialView("_PaypalPartial", cart);
        }

        [HttpPost]
        public void PlaceOrder()
        {
            var cart = _session.Get<List<CartVM>>("cart") ?? new List<CartVM>();
            // Получить имя пользователя и Id пользователя
            var userName = User.Identity.Name;
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);
            // Заполняем OrderDTO
            var order = new OrderDTO()
            {
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };

            foreach (var item in cart)
            {
                db.OrderDetails.Add(new OrderDetailsDTO
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }
                );
            }

            db.Orders.Add(order);
            db.SaveChanges();

            // отправить письмо на почту Admin
            Emailer.SendOrderMail(order.Id, cart);
            // Обнулить сессию
            _session.Set<CartVM>("cart", null);
        }
    }
}
