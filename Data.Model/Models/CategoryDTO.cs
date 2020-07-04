using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model.Models
{
    [Table("Categories")]
    public class CategoryDTO
    {
        public CategoryDTO() { Id = Guid.NewGuid(); }
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }

        public IEnumerable<ProductDTO> Products { get; set; }
    }
}
