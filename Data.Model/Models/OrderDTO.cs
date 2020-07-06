using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model.Models
{
    [Table("Orders")]
    public class OrderDTO
    {
        public OrderDTO() { Id = Guid.NewGuid(); }
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Id")]
        public AppUser User { get; set; }
        [ForeignKey("Id")]
        public List<OrderDetailsDTO> Products { get; set; }
    }
}
