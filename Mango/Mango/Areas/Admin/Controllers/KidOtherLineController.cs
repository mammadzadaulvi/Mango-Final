using Mango.Areas.Admin.ViewModels.KidOtherLine;
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
    public class KidOtherLineController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public KidOtherLineController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new KidOtherLineIndexVM
            {
                KidOtherLines = await _appDbContext.KidOtherLines.ToListAsync()
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
        public async Task<IActionResult> Create(KidOtherLineCreateVM model)
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

            var kidOtherLine = new KidOtherLine
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.KidOtherLines.AddAsync(kidOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Update 

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var kidOtherLine = await _appDbContext.KidOtherLines.FindAsync(id);

            if (kidOtherLine == null) return NotFound();

            var model = new KidOtherLineUpdateVM
            {
                Id = kidOtherLine.Id,
                Title = kidOtherLine.Title,
                Url = kidOtherLine.Url,
                PhotoPath = kidOtherLine.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(KidOtherLineUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var kidOtherLine = await _appDbContext.KidOtherLines.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (kidOtherLine == null) return NotFound();

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

                _fileService.Delete(kidOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);
                kidOtherLine.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            kidOtherLine.Title = model.Title;
            kidOtherLine.Url = model.Url;
            model.PhotoPath = kidOtherLine.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var kidOtherLine = await _appDbContext.KidOtherLines.FindAsync(id);
            if (kidOtherLine == null) return NotFound();

            _fileService.Delete(kidOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.KidOtherLines.Remove(kidOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var kidOtherLine = await _appDbContext.KidOtherLines.FindAsync(id);
            if (kidOtherLine == null) return NotFound();

            var model = new KidOtherLineDetailsVM
            {
                Id = kidOtherLine.Id,
                Title = kidOtherLine.Title,
                Url = kidOtherLine.Url,
                PhotoPath = kidOtherLine.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
