using Data.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Areas.Admin.Models
{
    public class CategoryVM
    {
        public CategoryVM() { }
        public CategoryVM(CategoryDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Slug = row.Slug;
            Sorting = row.Sorting;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int Sorting { get; set; }

        public CategoryDTO ToDbModel(CategoryDTO src)
        {
            var res = src ?? new CategoryDTO();

            res.Name = this.Name;
            res.Slug = this.Slug;
            res.Sorting = this.Sorting;

            return res;
        }

    }
}
