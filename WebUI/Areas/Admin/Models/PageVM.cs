using Data.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Areas.Admin.Models
{
    public class PageVM
    {
        public PageVM() { }
        public PageVM(PagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Slug = row.Slug;
            Body = row.Body;
            Sorting = row.Sorting;
            HasSidebar = row.HasSidebar;
        }
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Требуется поле Title")]
        [StringLength(50, MinimumLength = 3,ErrorMessage ="Неверная длина поля")]
        public string Title { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage ="Требуется поле Body")]
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "Неверная длина поля")]
        public string Body { get; set; }
        public int Sorting { get; set; }
        [Display(Name = "Sidebar")]
        public bool HasSidebar { get; set; }

        public void Normalize()
        {
            this.Title = this.Title.ToUpper();

            // Объявляем пременную для краткого описания (slug)
            string slug;
            // Проверяем, есть ли краткое описание, если нет присваиваем его
            if (string.IsNullOrWhiteSpace(this.Slug))
            {
                slug = this.Title;
            }
            else
            {
                slug = this.Slug;
            }
            this.Slug = slug.Replace(" ", "-").ToLower();
        }

        public PagesDTO ToDbModel(PagesDTO src)
        {
             PagesDTO res = src ?? new PagesDTO();
            if (src == null)
            {
                res = new PagesDTO();
                res.Id = Guid.NewGuid();
                res.Sorting = 100;
            }
            else
            {
                res = src;
            }

            res.Title = this.Title;
            res.Slug = this.Slug;
            res.Body = this.Body;
            res.Sorting = this.Sorting;
            res.HasSidebar = this.HasSidebar;

            return res;
        }
    }
}
