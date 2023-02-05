using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Areas.Admin.ViewModels.ProductSubCategory
{
    public class ProductSubCategoryIndexVM
    {
        public List<Models.ProductSubCategory> ProductSubCategories { get; set; }  
        public List<SelectListItem> ProductCategories { get; set; } 
    }
}
