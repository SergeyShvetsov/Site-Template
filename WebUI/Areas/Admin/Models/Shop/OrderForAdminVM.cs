using System;
using System.Collections.Generic;
using System.Linq;

namespace WebUI.Areas.Admin.Models.Shop
{
    public class OrderForAdminVM
    {
        public Guid OrderId { get; set; }
        public string UserName { get; set; }
        public List<OrderItemVM> Products { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal Total => Products.Sum(x => x.Total);
    }
    public class OrderItemVM
    {
        public string ProductName { get; set; }
        public int Quantyty { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Price * Quantyty;
    }
}
