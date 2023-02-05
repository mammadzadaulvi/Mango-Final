using Mango.Constants;
using Mango.DAL;
using Mango.Helpers;
using Mango.Models;
using Mango.ViewModels.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Mango.Controllers
{
    public class SaleController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public SaleController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        #region Filter

        private IQueryable<Product> FilterProducts(SaleIndexVM model)
        {
            var products = FilterByTitle(model.Title);

            products = FilterByRefCode(model.RefCode);
          
            //products = FilterByGender(model.Gender);
            
            //products = FilterBySize(model.Size);

            products = FilterByCategory(products, model.SubCategoryId);

            products = FilterByPrice(products, model.MinPrice, model.MaxPrice);

            return products;
        }

        private IQueryable<Product> FilterByTitle(string title)
        {
            return _appDbContext.Products.Where(p => !string.IsNullOrEmpty(title) ? p.Title.Contains(title) : true);
        }

        private IQueryable<Product> FilterByRefCode(string refcode)
        {
            return _appDbContext.Products.Where(p => !string.IsNullOrEmpty(refcode) ? p.Title.Contains(refcode) : true);
        }


        //private IQueryable<Product> FilterByGender(Gender gender)
        //{
        //    //return _appDbContext.Products.Where(p => !string.IsNullOrEmpty(gender) ? p.Title.Contains(gender) : true);

        //    throw new Exception(); 

        //}

        private IQueryable<Product> FilterBySize(string size)
        {
            return _appDbContext.Products.Where(p => !string.IsNullOrEmpty(size) ? p.Title.Contains(size) : true);
        }

        private IQueryable<Product> FilterByCategory(IQueryable<Product> products, int? categoryId)
        {
            return products.Where(p => categoryId != null ? p.ProductSubCategoryId == categoryId : true);
        }

        private IQueryable<Product> FilterBySubCategory(IQueryable<Product> products, int? SubCategoryId)
        {
            return products.Where(p => SubCategoryId != null ? p.ProductSubCategoryId == SubCategoryId : true);
        }
        private IQueryable<Product> FilterByPrice(IQueryable<Product> products, double? minPrice, double? maxPrice)
        {
            return products.Where(p => (minPrice != null ? p.Price >= minPrice : true) && (maxPrice != null ? p.Price <= maxPrice : true));
        }


        #endregion


        [HttpGet]
        public async Task<IActionResult> Index(SaleIndexVM model,string filter, 
                                               int? SubCategoryId = null, string? title = null, 
                                               string? refCode = null, double? minPrice = 0,
                                               double? maxPrice = null, int? sizeId = null, int page=1)
        {
            //var products = FilterProducts(model);
            var products = _appDbContext.Products.AsQueryable();
              
            if(title != null) products = products.Where(p => p.Title.ToLower().Contains(title.ToLower()));
            if(refCode != null) products = products.Where(p => p.RefCode.ToLower().Contains(refCode.ToLower()));
            if(minPrice != null ) products = products.Where(p => p.Price >= minPrice);
            if(maxPrice != null ) products = products.Where(p => p.Price <= maxPrice);
            if (sizeId != null) products = products.Where(p => p.ProductSizes.Any(x => x.SizeId == sizeId));
            if(SubCategoryId != null) products = products.Where(p => p.ProductSubCategoryId == SubCategoryId);

            ViewBag.Filter = filter;
            var query = products.Where(x => x.ProductCategory.Title == filter).Include(p => p.ProductPhotos.OrderBy(his => his.Order));

            model = new SaleIndexVM
            {
                ProductCategories = await _appDbContext.ProductCategories.ToListAsync(),
                ProductSubCategories = await _appDbContext.ProductSubCategories.ToListAsync(),
                Sizes = _appDbContext.Sizes.ToList(),
                Products =  PaginatedList<Product>.Create(query, 8, page),
            };

            return View(model);
        }


        public async Task<IActionResult> Details(int id)
        {
            //var product = await _appDbContext.Products.Include(p => p.ProductPhotos).ToListAsync();
            var product = await _appDbContext.Products.Include(p => p.ProductPhotos).Include(p => p.ProductSizes).ThenInclude(x=>x.Size).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var model = new SaleDetailsVM
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                RefCode = product.RefCode,
                Color = product.Color,
                Description = product.Description,
                Material = product.Material,
                MaterialIcon = product.MaterialIcon,
                ProductPhotos = product.ProductPhotos,
                Sizes = product.ProductSizes.Select(x=> x.Size).ToList(),
                ProductSizeIds = product.ProductSizes.Select(p => p.SizeId).ToList()
            };

            return View(model);
        }
    }
}
