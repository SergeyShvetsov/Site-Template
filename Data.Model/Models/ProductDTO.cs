using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model.Models
{
    [Table("Products")]
    public class ProductDTO
    {
        public ProductDTO() { Id = Guid.NewGuid(); }

        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public string ImageName { get; set; }

        // назначаем внешний ключ
        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }
    }
}
