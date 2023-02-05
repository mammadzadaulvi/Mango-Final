using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Areas.Admin.ViewModels.ProductSubCategory
{
    public class ProductSubCategoryUpdateVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ProductCategoryId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}
