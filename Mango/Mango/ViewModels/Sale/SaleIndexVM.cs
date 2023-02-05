using Mango.Constants;
using Mango.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Mango.ViewModels.Sale
{
    public class SaleIndexVM
    {
        public List<Models.ProductCategory> ProductCategories { get; set; }
        public List<Models.ProductSubCategory> ProductSubCategories { get; set; }
        public List<Models.Size> Sizes { get; set; }
        public PaginatedList<Models.Product> Products { get; set; }

        public string? Title { get; set; }
        public string? RefCode { get; set; }
        //public string? Gender { get; set; }
        //public string? Size { get; set; }
        public List<SelectListItem> SubCategories { get; set; }

        [Display(Name = "Category")]
        public int? SubCategoryId { get; set; }



        [Display(Name = "Minimum Price")]
        public double? MinPrice { get; set; }

        [Display(Name = "Maximum Price")]
        public double? MaxPrice { get; set; }
        public int? SizeId { get; set; }
        public Gender Gender { get; set; }

    }
}
