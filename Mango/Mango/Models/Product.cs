namespace Mango.Models
{
    public class Product
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
        public int? ProductCategoryId { get; set; }
        public ProductSubCategory ProductSubCategory { get; set; }
        public ICollection<ProductPhoto> ProductPhotos { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public List<ProductSize> ProductSizes { get; set; }


    }
}
