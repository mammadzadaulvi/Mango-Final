using Mango.Areas.Admin.ViewModels.Product;
using Mango.Areas.Admin.ViewModels.Product.ProductPhoto;
using Mango.DAL;
using Mango.Helpers;
using Mango.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing;

namespace Mango.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }


        public async Task<IActionResult> Index(ProductIndexVM model,int page=1)
        {
            var query =  _appDbContext.Products.Include(p => p.ProductSubCategory).AsQueryable();
            model = new ProductIndexVM
            {
                Products = PaginatedList<Product>.Create(query,5,page),
                ProductSubCategories = await _appDbContext.ProductSubCategories.Select(c => new SelectListItem
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
            var model = new ProductCreateVM
            {
                Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync(),

                SubCategories = await _appDbContext.ProductSubCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {

            model.Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            model.SubCategories = await _appDbContext.ProductSubCategories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (!ModelState.IsValid) return View(model);

            var category = await _appDbContext.ProductSubCategories.FindAsync(model.ProductSubCategoryId);

            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Category not found");
                return View(model);
            }

            bool isExist = await _appDbContext.Products.AnyAsync(p => p.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "This category is already exist");
                return View(model);
            }


            bool hasError = false;
            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} yuklediyiniz file sekil formatinda olmalidir");
                    hasError = true;

                }
                else if (!_fileService.CheckSize(photo, 300))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} ci yuklediyiniz sekil 300 kb dan az olmalidir");
                    hasError = true;

                }

            }

            if (hasError) return View(model);

            if (model.CreateAt == null)
            {
                model.CreateAt = DateTime.Today;
            }

            var product = new Product
            {
                Title = model.Title,
                Price = model.Price,
                RefCode = model.RefCode,
                Color = model.Color,
                Size = model.Size,
                Description = model.Description,
                Material = model.Material,
                MaterialIcon = model.MaterialIcon,
                ProductSubCategoryId = model.ProductSubCategoryId,
                ProductCategoryId = model.ProductCategoryId,
                CreateAt = model.CreateAt
                //PhotoName = await _fileService.UploadAsync(model.MainPhoto, _webHostEnvironment.WebRootPath),
            };

            await _appDbContext.Products.AddAsync(product);
            await _appDbContext.SaveChangesAsync();


            int order = 1;
            foreach (var photo in model.Photos)
            {
                var productPhoto = new ProductPhoto
                {
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order,
                    ProductId = product.Id
                };
                await _appDbContext.ProductPhotos.AddAsync(productPhoto);
                await _appDbContext.SaveChangesAsync();

                order++;
            }

            return RedirectToAction("Index");

        }
        #endregion

        #region Update
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _appDbContext.Products.Include(hs => hs.ProductPhotos).FirstOrDefaultAsync(hs => hs.Id == id);
            //var product = await _appDbContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            var model = new ProductUpdateVM
            {
                Title = product.Title,
                Price = product.Price,
                RefCode = product.RefCode,
                Color = product.Color,
                Size = product.Size,
                Description = product.Description,
                Material = product.Material,
                MaterialIcon = product.MaterialIcon,
                ProductSubCategoryId = product.ProductSubCategoryId,
                ProductCategoryId = (int)product.ProductCategoryId,
                ProductPhotos = product.ProductPhotos,

                Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync(),


                SubCategories = await _appDbContext.ProductSubCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM model, int id)
        {
            model.Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();


            model.SubCategories = await _appDbContext.ProductSubCategories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToListAsync();

            if (id != model.Id) return View(model);
            if (id != model.Id) return BadRequest();


            var product = await _appDbContext.Products.Include(hs => hs.ProductPhotos).FirstOrDefaultAsync(hs => hs.Id == id);
            model.ProductPhotos = product.ProductPhotos.ToList();

            if (product == null) return NotFound();

            bool isExist = await _appDbContext.Products.AnyAsync(p => p.Title.ToLower().Trim() == product.Title.ToLower().Trim() && p.Id != product.Id);

            if (isExist)
            {
                ModelState.AddModelError("Title", "This product is already exist");
                return View(model);
            }


            bool hasError = false;

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} must be image format");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(photo, 300))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} must be lesser than 300 kb");
                        hasError = true;
                    }
                }

                if (hasError) { return View(model); }

                var productPhoto = product.ProductPhotos.OrderByDescending(hs => hs.Order).FirstOrDefault();

                int order = productPhoto != null ? productPhoto.Order : 0;
                foreach (var photo in model.Photos)
                {
                    var prodPhoto = new ProductPhoto
                    {
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = ++order,
                        ProductId = product.Id
                    };
                    await _appDbContext.ProductPhotos.AddAsync(prodPhoto);
                    await _appDbContext.SaveChangesAsync();
                }
            }


            var category = await _appDbContext.ProductSubCategories.FindAsync(model.ProductSubCategoryId);
            if (category == null) return NotFound();

            model.ProductSubCategoryId = category.Id;

            product.Title = model.Title;
            product.Price = model.Price;
            product.RefCode = model.RefCode;
            product.Color = model.Color;
            product.Size = model.Size;
            product.Description = model.Description;
            product.Material = model.Material;
            product.MaterialIcon = model.MaterialIcon;
            product.CreateAt = model.CreateAt;
            product.ProductSubCategoryId = model.ProductSubCategoryId;
            product.ProductCategoryId = model.ProductCategoryId;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        #endregion



        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _appDbContext.Products.Include(hs => hs.ProductPhotos).FirstOrDefaultAsync(hs => hs.Id == id);

            //var product = await _appDbContext.Products.FindAsync(id);

            if (product == null) return NotFound();

            //_fileService.Delete(product.PhotoName, _webHostEnvironment.WebRootPath);

            foreach (var photo in product.ProductPhotos)
            {
                _fileService.Delete(photo.Name, _webHostEnvironment.WebRootPath);
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion

        #region Details
        public async Task<IActionResult> Details(int id)
        {
            var product = await _appDbContext.Products.Include(hs => hs.ProductPhotos).FirstOrDefaultAsync(hs => hs.Id == id);

            //var product = await _appDbContext.Products.FindAsync(id);

            if (product == null) return NotFound();

            var model = new ProductDetailsVM
            {
                Id = product.Id,
                Title = product.Title,
                RefCode = product.RefCode,
                Color = product.Color,
                Size = product.Size,
                Description = product.Description,
                Material = product.Material,
                MaterialIcon = product.MaterialIcon,
                CreateAt = product.CreateAt,
                ProductSubCategoryId = product.ProductSubCategoryId,

                Categories = await _appDbContext.ProductCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync(),

                SubCategories = await _appDbContext.ProductSubCategories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync(),
                Photos = product.ProductPhotos
            };
            return View(model);
        }
        #endregion









        #region UpdatePhoto

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (productPhoto == null) return NotFound();

            var model = new ProductPhotoUpdateVM
            {
                Order = productPhoto.Order
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, ProductPhotoUpdateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();

            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(model.Id);
            if (productPhoto == null) return NotFound();

            productPhoto.Order = model.Order;
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "Product", new { id = productPhoto.ProductId });

        }

        #endregion

        #region DeletePhoto

        [HttpGet]
        public async Task<IActionResult> Deletephoto(int id)
        {
            var productPhoto = await _appDbContext.ProductPhotos.FindAsync(id);
            if (productPhoto == null) return NotFound();

            _fileService.Delete(productPhoto.Name, _webHostEnvironment.WebRootPath);

            _appDbContext.ProductPhotos.Remove(productPhoto);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "Product", new { id = productPhoto.ProductId });
        }

        #endregion


    }
}
