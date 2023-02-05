using Mango.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Mango.Areas.Admin.ViewModels.Product
{
    public class ProductIndexVM
    {
        public PaginatedList<Models.Product> Products { get; set; }   
        public List<SelectListItem> ProductSubCategories { get; set; }

        
    }
}
