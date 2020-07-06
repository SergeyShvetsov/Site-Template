using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Data.Tools
{
    public class Emailer
    {
        public static bool SendOrderMail(Guid orderId)
        {
            // отправить письмо на почту Admin
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("449eb1aa358e46", "299db081ab95dc"),
                EnableSsl = true
            };
            client.Send("shop@example.com", "admin@example.com", "New Order", 
                $"You have a new order [{orderId}]");
            return true;
        }
    }
}
