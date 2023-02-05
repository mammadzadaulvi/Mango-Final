using Mango.Areas.Admin.ViewModels.ProductSubCategory;
using Mango.DAL;
using Mango.Helpers;
using Mango.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mango.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductSubCategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductSubCategoryController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index(ProductSubCategoryIndexVM model)
        {
            model = new ProductSubCategoryIndexVM()
            {
                ProductSubCategories = await _appDbContext.ProductSubCategories.Include(p => p.ProductCategory).ToListAsync(),
                ProductCategories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                })
               .ToListAsync()

            };
            return View(model);
        }


        #region Create

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductSubCategoryCreateVM
            {
                Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Create(ProductSubCategoryCreateVM model)
        {
            model.Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (!ModelState.IsValid) return View(model);
            var category = await _appDbContext.ProductCategories.FindAsync(model.ProductCategoryId);

            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(model);
            }

            bool isExist = await _appDbContext.ProductSubCategories.AnyAsync(p => p.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "This category is already exist");
                return View(model);
            }

            //if (!_fileService.IsImage(model.MainPhoto))
            //{
            //    ModelState.AddModelError("MainPhoto", "The image must be img format");
            //    return View(model);
            //}
            //if (!_fileService.CheckSize(model.MainPhoto, 300))
            //{
            //    ModelState.AddModelError("MainPhoto", "This image is bigger than 300kb");
            //    return View(model);
            //}



            var productSubCategory = new ProductSubCategory
            {
                Title = model.Title,
                ProductCategoryId = model.ProductCategoryId

            };

            await _appDbContext.ProductSubCategories.AddAsync(productSubCategory);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var productSubCategory = await _appDbContext.ProductSubCategories.FindAsync(id);

            if (productSubCategory == null) return NotFound();

            var model = new ProductSubCategoryUpdateVM
            {
                Title = productSubCategory.Title,
                ProductCategoryId = productSubCategory.ProductCategoryId,

                Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductSubCategoryUpdateVM model, int id)
        {
            model.Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (id != model.Id) return View(model);
            if (id != model.Id) return BadRequest();

            var productSubCategory = await _appDbContext.ProductSubCategories.FindAsync(id);


            if (productSubCategory == null) return NotFound();
            bool isExist = await _appDbContext.ProductSubCategories.AnyAsync(p => p.Title.ToLower().Trim() == productSubCategory.Title.ToLower().Trim() && p.Id != productSubCategory.Id);

            if (isExist)
            {
                ModelState.AddModelError("Title", "This product is already exist");
                return View(model);
            }


            //if (model.MainPhoto != null)
            //{

            //    if (!_fileService.IsImage(model.MainPhoto))
            //    {
            //        ModelState.AddModelError("Photo", "The image must be img format");
            //        return View(model);
            //    }
            //    if (!_fileService.CheckSize(model.MainPhoto, 300))
            //    {
            //        ModelState.AddModelError("Photo", "The image is bigger than 300kb");
            //        return View(model);
            //    }


            //    _fileService.Delete(model.MainPhotoPath, _webHostEnvironment.WebRootPath);
            //    model.MainPhotoPath = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath);
            //    product.PhotoName = model.MainPhotoPath;
            //}

            var productCategory = await _appDbContext.ProductCategories.FindAsync(model.ProductCategoryId);
            if (productCategory == null) return NotFound();
            model.ProductCategoryId = productCategory.Id;

            productSubCategory.Title = model.Title;
            productSubCategory.ProductCategoryId = model.ProductCategoryId;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var productSubCategory = await _appDbContext.ProductSubCategories.FindAsync(id);
            if (productSubCategory == null) return NotFound();

            //_fileService.Delete(product.PhotoName, _webHostEnvironment.WebRootPath);

            _appDbContext.ProductSubCategories.Remove(productSubCategory);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Details
        public async Task<IActionResult> Details(int id)
        {
            var productSubCategory = await _appDbContext.ProductSubCategories.FindAsync(id);
            if (productSubCategory == null) return NotFound();

            var model = new ProductSubCategoryDetailsVM
            {
                Id = productSubCategory.Id,
                Title = productSubCategory.Title,
                ProductCategoryId = productSubCategory.ProductCategoryId,

                Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }
        #endregion
    }
}
