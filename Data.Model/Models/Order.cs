using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Model.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public string User { get; set; } // имя фамилия покупателя
        public string Address { get; set; } // адрес покупателя
        public string ContactPhone { get; set; } // контактный телефон покупателя

        public Guid PhoneId { get; set; } // ссылка на связанную модель Phone
        public Phone Phone { get; set; }
    }
}
