using Data.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Areas.Admin.Models
{
    public class SidebarVM
    {
        public SidebarVM() {}
        public SidebarVM(SidebarDTO row)
        {
            Id = row.Id;
            Body = row.Body;
        }

        public Guid Id { get; set; }
        public string Body { get; set; }

        public SidebarDTO ToDbModel(SidebarDTO src)
        {
            var res = src ?? new SidebarDTO();

            res.Body = this.Body;

            return res;
        }

    }
}
