namespace Mango.Models
{
    public class ProductSubCategory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
