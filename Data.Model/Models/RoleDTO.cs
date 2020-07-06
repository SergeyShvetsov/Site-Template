using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model.Models
{
    [Table("Roles")]
    public class RoleDTO
    {
        public RoleDTO() 
        {
            Id = Guid.NewGuid();
            RoleType = RoleType.User;
        }

        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RoleType RoleType { get; set; }
    }
}
