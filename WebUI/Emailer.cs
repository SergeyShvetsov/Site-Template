using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using WebUI.Models.Cart;

namespace WebUI
{
    public class Emailer
    {
        public static bool SendOrderMail(Guid orderId, IEnumerable<CartVM> details)
        {
            // отправить письмо на почту Admin
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("449eb1aa358e46", "299db081ab95dc"),
                EnableSsl = true
            };

            var sb = new StringBuilder();

            sb.Append($"You have a new order [{orderId}]\n\r");
            foreach(var itm in details)
            {
                sb.Append($"\n\r{itm.Name} - {itm.Price}$ x {itm.Quantity} = {itm.Total}$");
            }
            client.Send("shop@example.com", "admin@example.com", "New Order", sb.ToString());
            return true;
        }
    }
}
