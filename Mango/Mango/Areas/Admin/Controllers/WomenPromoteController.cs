using Mango.Areas.Admin.ViewModels.WomenPromote;
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
    public class WomenPromoteController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WomenPromoteController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            var query = _appDbContext.WomenPromotes.AsQueryable();
            var model = new WomenPromoteIndexVM
            {
                WomenPromotes = PaginatedList<WomenPromote>.Create(query,5,page)
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
        public async Task<IActionResult> Create(WomenPromoteCreateVM model)
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

            var womenPromote = new WomenPromote
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.WomenPromotes.AddAsync(womenPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update 


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var womenPromote = await _appDbContext.WomenPromotes.FindAsync(id);

            if (womenPromote == null) return NotFound();

            var model = new WomenPromoteUpdateVM
            {
                Id = womenPromote.Id,
                Title = womenPromote.Title,
                Url = womenPromote.Url,
                PhotoPath = womenPromote.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(WomenPromoteUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var womenPromote = await _appDbContext.WomenPromotes.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (womenPromote == null) return NotFound();

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

                _fileService.Delete(womenPromote.PhotoPath, _webHostEnvironment.WebRootPath);
                womenPromote.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            womenPromote.Title = model.Title;
            womenPromote.Url = model.Url;
            model.PhotoPath = womenPromote.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var womenPromote = await _appDbContext.WomenPromotes.FindAsync(id);
            if (womenPromote == null) return NotFound();

            _fileService.Delete(womenPromote.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.WomenPromotes.Remove(womenPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var womenPromote = await _appDbContext.WomenPromotes.FindAsync(id);
            if (womenPromote == null) return NotFound();

            var model = new WomenPromoteDetailsVM
            {
                Id = womenPromote.Id,
                Title = womenPromote.Title,
                Url = womenPromote.Url,
                PhotoPath = womenPromote.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
