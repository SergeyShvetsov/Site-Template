using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model.Models
{
    [Table("OrderDetails")]
    public class OrderDetailsDTO
    {
        public OrderDetailsDTO() { Id = Guid.NewGuid(); }
        [Key]
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal Total => Price * Quantity;

        [ForeignKey("OrderId")]
        public OrderDTO Order { get; set; }
        [ForeignKey("ProductId")]
        public ProductDTO Product { get; set; }
    }
}
