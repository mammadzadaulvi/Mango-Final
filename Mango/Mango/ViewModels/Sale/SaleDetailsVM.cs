using Mango.Models;

namespace Mango.ViewModels.Sale
{
    public class SaleDetailsVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public string RefCode { get; set; }
        public string Color { get; set; }
        public int SizeId { get; set; }
        public string Description { get; set; }
        public string Material { get; set; }
        public string MaterialIcon { get; set; }

        public int SubCategoryId { get; set; }

        //Yeni/////

        //public Product Product { get; set; }

        public ProductSubCategory ProductSubCategory { get; set; }
        public ICollection<ProductPhoto> ProductPhotos { get; set; }
        public List<ProductSubCategory> ProductSubCategories { get; set; }
        public List<Size> Sizes { get; set; }
        public List<int> ProductSizeIds { get; set; }
    }
}
