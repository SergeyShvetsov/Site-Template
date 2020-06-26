using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model.Models
{
    [Table("Sidebar")]
    public class SidebarDTO
    {
        public SidebarDTO() { Id = Guid.NewGuid(); }
        [Key]
        public Guid Id { get; set; }
        public string Body { get; set; }
    }
}
