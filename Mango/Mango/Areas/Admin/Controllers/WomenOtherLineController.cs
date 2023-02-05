using Mango.Areas.Admin.ViewModels.WomenOtherLine;
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
    public class WomenOtherLineController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WomenOtherLineController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new WomenOtherLineIndexVM
            {
                WomenOtherLines = await _appDbContext.WomenOtherLines.ToListAsync()
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
        public async Task<IActionResult> Create(WomenOtherLineCreateVM model)
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

            var womenOtherLine = new WomenOtherLine
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.WomenOtherLines.AddAsync(womenOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Update 

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var womenOtherLine = await _appDbContext.WomenOtherLines.FindAsync(id);

            if (womenOtherLine == null) return NotFound();

            var model = new WomenOtherLineUpdateVM
            {
                Id = womenOtherLine.Id,
                Title = womenOtherLine.Title,
                Url = womenOtherLine.Url,
                PhotoPath = womenOtherLine.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(WomenOtherLineUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var womenOtherLine = await _appDbContext.WomenOtherLines.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (womenOtherLine == null) return NotFound();

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

                _fileService.Delete(womenOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);
                womenOtherLine.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            womenOtherLine.Title = model.Title;
            womenOtherLine.Url = model.Url;
            model.PhotoPath = womenOtherLine.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var womenOtherLine = await _appDbContext.WomenOtherLines.FindAsync(id);
            if (womenOtherLine == null) return NotFound();

            _fileService.Delete(womenOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.WomenOtherLines.Remove(womenOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var womenOtherLine = await _appDbContext.WomenOtherLines.FindAsync(id);
            if (womenOtherLine == null) return NotFound();

            var model = new WomenOtherLineDetailsVM
            {
                Id = womenOtherLine.Id,
                Title = womenOtherLine.Title,
                Url = womenOtherLine.Url,
                PhotoPath = womenOtherLine.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
