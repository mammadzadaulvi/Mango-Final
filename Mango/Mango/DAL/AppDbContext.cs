using Mango.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace Mango.DAL
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }


        public DbSet<WomenIntro> WomenIntro { get; set; }
        public DbSet<WomenMangoGirl> WomenMangoGirls { get; set; }
        public DbSet<WomenOtherLine> WomenOtherLines { get; set; }
        public DbSet<WomenPromote> WomenPromotes { get; set; }


        public DbSet<MenIntro> MenIntro { get; set; }
        public DbSet<MenOtherLine> MenOtherLines { get; set; }
        public DbSet<MenPromote> MenPromotes { get; set; }


        public DbSet<TeenIntro> TeenIntro { get; set; }
        public DbSet<TeenOtherLine> TeenOtherLines { get; set; }
        public DbSet<TeenPromote> TeenPromotes { get; set; }


        public DbSet<KidIntro> KidIntro { get; set; }
        public DbSet<KidOtherLine> KidOtherLines { get; set; }
        public DbSet<KidPromote> KidPromotes { get; set; }


        public DbSet<HomeIntro> HomeIntro { get; set; }
        public DbSet<HomeOtherLine> HomeOtherLines { get; set; }
        public DbSet<HomePromote> HomePromotes { get; set; }



        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductSubCategory> ProductSubCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }

        public DbSet<Size> Sizes { get; set; }
    }
}
