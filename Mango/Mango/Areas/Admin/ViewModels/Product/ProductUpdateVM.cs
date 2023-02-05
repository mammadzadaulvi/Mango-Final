﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Areas.Admin.ViewModels.Product
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string RefCode { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public string Material { get; set; }
        public string MaterialIcon { get; set; }
        public DateTime CreateAt { get; set; }


        public int ProductSubCategoryId { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public ICollection<Models.ProductPhoto>? ProductPhotos { get; set; } 
        public List<SelectListItem>? SubCategories { get; set; }

        public int ProductCategoryId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}
