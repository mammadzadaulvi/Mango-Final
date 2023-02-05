using Mango.Areas.Admin.ViewModels.ProductCategory;
using Mango.DAL;
using Mango.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mango.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductCategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductCategoryController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ProductCategoryIndexVM
            {
                ProductCategories = await _appDbContext.ProductCategories.ToListAsync()
            };
            return View(model);
        }





        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);

            bool isExist = await _appDbContext.ProductCategories
                            .AnyAsync(c => c.Title.ToLower().Trim() == model.Title.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Title", "This title is already exist");
                return View(model);
            }

            var productCategory = new ProductCategory
            {
                Title = model.Title
            };

            await _appDbContext.ProductCategories.AddAsync(productCategory);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        #endregion

        #region Update

        public async Task<IActionResult> Update(int id)
        {
            var category = await _appDbContext.ProductCategories.FindAsync(id);
            if (category == null) return NotFound();

            ProductCategoryUpdateVM productCategoryUpdateViewModel = new ProductCategoryUpdateVM
            {
                Id = category.Id,
                Title = category.Title,
            };

            return View(productCategoryUpdateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductCategoryUpdateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            var dbCategory = await _appDbContext.ProductCategories.FindAsync(id);
            if (dbCategory == null) return NotFound();

            bool isExist = await _appDbContext.ProductCategories.AnyAsync(ct => ct.Title.ToLower().Trim() == model.Title.ToLower().Trim() && ct.Id != model.Id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "This component is already exist");
                return View(model);
            }

            dbCategory.Title = model.Title;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dbCategory = await _appDbContext.ProductCategories.FindAsync(id);
            if (dbCategory == null) return NotFound();

            _appDbContext.ProductCategories.Remove(dbCategory);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var productCategory = await _appDbContext.ProductCategories.FindAsync(id);
            if (productCategory == null) return NotFound();
            return View(productCategory);
        }

        #endregion
    }
}
