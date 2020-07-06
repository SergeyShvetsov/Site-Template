using Data.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Areas.Admin.Models.Shop
{
    public class OrderVM
    {
        public OrderVM() { }
        public OrderVM(OrderDTO row)
        {
            Id = row.Id;
            UserId = row.UserId;
            CreatedAt = row.CreatedAt;
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
