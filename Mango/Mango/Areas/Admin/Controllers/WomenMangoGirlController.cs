using Mango.Areas.Admin.ViewModels.WomenMangoGirl;
using Mango.DAL;
using Mango.Helpers;
using Mango.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mango.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class WomenMangoGirlController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WomenMangoGirlController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new WomenMangoGirlIndexVM
            {
                WomenMangoGirls = await _appDbContext.WomenMangoGirls.ToListAsync()
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
        public async Task<IActionResult> Create(WomenMangoGirlCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Yüklənən fayl image formatında olmalıdır.");
                return View(model);
            }
            if (!_fileService.CheckSize(model.Photo, 300))
            {
                ModelState.AddModelError("Photo", "Şəkilin ölçüsü 300 kb-dan böyükdür");
                return View(model);
            }

            var womenMangoGirl = new WomenMangoGirl
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.WomenMangoGirls.AddAsync(womenMangoGirl);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Update 

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var womenMangoGirl = await _appDbContext.WomenMangoGirls.FindAsync(id);

            if (womenMangoGirl == null) return NotFound();

            var model = new WomenMangoGirlUpdateVM
            {
                Id = womenMangoGirl.Id,
                Title = womenMangoGirl.Title,
                Url = womenMangoGirl.Url,
                PhotoPath = womenMangoGirl.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(WomenMangoGirlUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var womenMangoGirl = await _appDbContext.WomenMangoGirls.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (womenMangoGirl == null) return NotFound();

            if (model.Photo != null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Yüklənən fayl image formatında olmalıdır.");
                    return View(model);
                }
                if (!_fileService.CheckSize(model.Photo, 300))
                {
                    ModelState.AddModelError("Photo", "Şəkilin ölçüsü 300 kb-dan böyükdür");
                    return View(model);
                }

                _fileService.Delete(womenMangoGirl.PhotoPath, _webHostEnvironment.WebRootPath);
                womenMangoGirl.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            womenMangoGirl.Title = model.Title;
            womenMangoGirl.Url = model.Url;
            model.PhotoPath = womenMangoGirl.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var womenMangoGirl = await _appDbContext.WomenMangoGirls.FindAsync(id);
            if (womenMangoGirl == null) return NotFound();

            _fileService.Delete(womenMangoGirl.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.WomenMangoGirls.Remove(womenMangoGirl);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var womenMangoGirl = await _appDbContext.WomenMangoGirls.FindAsync(id);
            if (womenMangoGirl == null) return NotFound();

            var model = new WomenMangoGirlDetailsVM
            {
                Id = womenMangoGirl.Id,
                Title = womenMangoGirl.Title,
                Url = womenMangoGirl.Url,
                PhotoPath = womenMangoGirl.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
