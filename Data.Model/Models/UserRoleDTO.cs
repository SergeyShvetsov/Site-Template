using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Model.Models
{
    [Table("UserRoles")]
    public class UserRoleDTO
    {
        [Key,Column(Order = 0)]
        public Guid UserId { get; set; }
        [Key, Column(Order = 1)]
        public Guid RoleId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [ForeignKey("RoleId")]
        public RoleDTO Role { get; set; }
    }
}
