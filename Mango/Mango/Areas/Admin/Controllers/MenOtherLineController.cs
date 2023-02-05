using Mango.Areas.Admin.ViewModels.MenOtherLine;
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
    public class MenOtherLineController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenOtherLineController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new MenOtherLineIndexVM
            {
                MenOtherLines = await _appDbContext.MenOtherLines.ToListAsync()
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
        public async Task<IActionResult> Create(MenOtherLineCreateVM model)
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

            var menOtherLine = new MenOtherLine
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.MenOtherLines.AddAsync(menOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Update 

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var menOtherLine = await _appDbContext.MenOtherLines.FindAsync(id);

            if (menOtherLine == null) return NotFound();

            var model = new MenOtherLineUpdateVM
            {
                Id = menOtherLine.Id,
                Title = menOtherLine.Title,
                Url = menOtherLine.Url,
                PhotoPath = menOtherLine.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(MenOtherLineUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var menOtherLine = await _appDbContext.MenOtherLines.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (menOtherLine == null) return NotFound();

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

                _fileService.Delete(menOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);
                menOtherLine.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            menOtherLine.Title = model.Title;
            menOtherLine.Url = model.Url;
            model.PhotoPath = menOtherLine.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var menOtherLine = await _appDbContext.MenOtherLines.FindAsync(id);
            if (menOtherLine == null) return NotFound();

            _fileService.Delete(menOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.MenOtherLines.Remove(menOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var menOtherLine = await _appDbContext.MenOtherLines.FindAsync(id);
            if (menOtherLine == null) return NotFound();

            var model = new MenOtherLineDetailsVM
            {
                Id = menOtherLine.Id,
                Title = menOtherLine.Title,
                Url = menOtherLine.Url,
                PhotoPath = menOtherLine.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
