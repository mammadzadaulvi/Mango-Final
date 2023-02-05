using Mango.Areas.Admin.ViewModels.HomeOtherLine;
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
    public class HomeOtherLineController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeOtherLineController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new HomeOtherLineIndexVM
            {
                HomeOtherLines = await _appDbContext.HomeOtherLines.ToListAsync()
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
        public async Task<IActionResult> Create(HomeOtherLineCreateVM model)
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

            var homeOtherLine = new HomeOtherLine
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.HomeOtherLines.AddAsync(homeOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update 


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var homeOtherLine = await _appDbContext.HomeOtherLines.FindAsync(id);

            if (homeOtherLine == null) return NotFound();

            var model = new HomeOtherLineUpdateVM
            {
                Id = homeOtherLine.Id,
                Title = homeOtherLine.Title,
                Url = homeOtherLine.Url,
                PhotoPath = homeOtherLine.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(HomeOtherLineUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var homeOtherLine = await _appDbContext.HomeOtherLines.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (homeOtherLine == null) return NotFound();

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

                _fileService.Delete(homeOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);
                homeOtherLine.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            homeOtherLine.Title = model.Title;
            homeOtherLine.Url = model.Url;
            model.PhotoPath = homeOtherLine.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var homeOtherLine = await _appDbContext.HomeOtherLines.FindAsync(id);
            if (homeOtherLine == null) return NotFound();

            _fileService.Delete(homeOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.HomeOtherLines.Remove(homeOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var homeOtherLine = await _appDbContext.HomeOtherLines.FindAsync(id);
            if (homeOtherLine == null) return NotFound();

            var model = new HomeOtherLineDetailsVM
            {
                Id = homeOtherLine.Id,
                Title = homeOtherLine.Title,
                Url = homeOtherLine.Url,
                PhotoPath = homeOtherLine.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
