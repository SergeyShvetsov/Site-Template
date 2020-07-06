using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data.Model;
using Data.Model.Models;
using Data.Tools.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WebUI.Areas.Admin.Models.Shop;
using WebUI.Extensions;
using WebUI.Models.Account;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext db;
        public AccountController(ApplicationContext cntx)
        {
            db = cntx;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [HttpGet]
        //[ActionName("create-account")]
        public IActionResult CreateAccount()
        {
            return View("CreateAccount");
        }
        [HttpPost]
        //[ActionName("create-account")]
        public IActionResult CreateAccount(UserVM model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid) return View(model);

            //  Проверяем соответствие пароля
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match!");
                return View(model);
            }
            // Проверяем имя на уникальность
            if (db.Users.Any(a => a.UserName.Equals(model.UserName)))
            {
                ModelState.AddModelError("", $"Login {model.UserName} is taken!");
                model.UserName = string.Empty;
                return View(model);
            }
            // Создать экземпляр UserDTO
            var userDTO = new AppUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                UserName = model.UserName,
                Password = model.Password
            };

            // Добавить роль пользователю
            var userRoleDto = new UserRoleDTO()
            {
                UserId = userDTO.Id,
                RoleId = db.Roles.First(x => x.RoleType == RoleType.User).Id
            };

            db.Users.Add(userDTO);
            db.UserRoles.Add(userRoleDto);
            // Сохранить данные
            db.SaveChanges();

            // Записать сообщение
            TempData["SM"] = "You are registered and can login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {

            // Проверить авторизован ли пользователь
            var userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("UserProfile");
            }

            // Проверяем пользователя на валидность
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserVM model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid) return View(model);

            if (!db.Users.Any(x => x.UserName.Equals(model.UserName) && x.Password.Equals(model.Password)))
            {
                ModelState.AddModelError("", "Invalid login or password.");
                return View(model);
            }

            await Authenticate(model.UserName); // аутентификация
            return Redirect("~/");
        }
        private async Task Authenticate(string userName)
        {
            var userId = db.Users.FirstOrDefault(x => x.UserName == userName).Id;
            var roles = db.UserRoles.Where(x => x.UserId == userId);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            foreach (var role in roles)
            {
                // добавляем роли
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, db.Roles.Find(role.RoleId).RoleType.ToString()));
            }

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserProfile()
        {
            // получаем имя пользователя
            var userName = User.Identity.Name;
            // Получаем пользователя
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);

            return View(new UserProfileVM(user));
        }
        [HttpPost]
        [Authorize]
        public IActionResult UserProfile(UserProfileVM model)
        {
            // Проверяем модель на валидность
            if (!ModelState.IsValid) return View(model);
            //  Проверяем соответствие пароля
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password do not match!");
                    return View(model);
                }
            }


            // Получить экземпляр UserDTO
            var dto = db.Users.Find(model.Id);
            if (!dto.UserName.Equals(model.UserName))
            {
                // Проверяем имя на уникальность
                if (db.Users.Any(a => a.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", $"Login {model.UserName} already exist!");
                    model.UserName = string.Empty;
                    return View(model);
                }

            }

            dto.FirstName = model.FirstName;
            dto.LastName = model.LastName;
            dto.EmailAddress = model.EmailAddress;
            dto.UserName = model.UserName;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                dto.Password = model.Password;
            }
            // Сохранить данные
            db.SaveChanges();

            // Записать сообщение
            TempData["SM"] = "You have edited your profile.";

            if (User.Identity.Name != model.UserName)
            {
                return RedirectToAction("Logout");
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult Orders()
        {
            var ordersForAdmin = new List<OrderForAdminVM>();
            var userId = db.Users.FirstOrDefault(x => x.UserName.Equals(User.Identity.Name)).Id;

            foreach (var order in db.Orders.Where(w => w.UserId == userId).Select(x => new OrderVM(x)).ToList())
            {
                var products = new List<OrderItemVM>();
                var orderDetailsList = db.OrderDetails.Where(w => w.OrderId == order.Id).ToList();

                foreach (var orderDetail in orderDetailsList)
                {
                    var product = db.Products.FirstOrDefault(x => x.Id == orderDetail.ProductId);
                    products.Add(new OrderItemVM
                    {
                        ProductName = product.Name,
                        Price = orderDetail.Price,
                        Quantyty = orderDetail.Quantity
                    });
                }
                ordersForAdmin.Add(new OrderForAdminVM
                {
                    OrderId = order.Id,
                    CreatedAt = order.CreatedAt,
                    Products = products
                });
            }

            return View(ordersForAdmin);
        }
    }
}
