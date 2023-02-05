using Mango.Areas.Admin.ViewModels.WomenIntro;
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
    public class WomenIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WomenIntroController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new WomenIntroIndexVM
            {
                WomenIntro = await _appDbContext.WomenIntro.FirstOrDefaultAsync()
            };
            return View(model);
        }


        #region Create 

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var womenIntro = await _appDbContext.WomenIntro.FirstOrDefaultAsync();
            if (womenIntro != null) return NotFound();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(WomenIntroCreateVM model)
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

            var womenIntro = new WomenIntro
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.WomenIntro.AddAsync(womenIntro);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var womenIntro = await _appDbContext.WomenIntro.FindAsync(id);

            if (womenIntro == null) return NotFound();

            var model = new WomenIntroUpdateVM
            {
                Id = womenIntro.Id,
                Title = womenIntro.Title,
                Description = womenIntro.Description,
                Url = womenIntro.Url,
                PhotoPath = womenIntro.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(WomenIntroUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var womenIntro = await _appDbContext.WomenIntro.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (womenIntro == null) return NotFound();

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

                _fileService.Delete(womenIntro.PhotoPath, _webHostEnvironment.WebRootPath);
                womenIntro.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            womenIntro.Title = model.Title;
            womenIntro.Description = model.Description;
            womenIntro.Url = model.Url;
            model.PhotoPath = womenIntro.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var womenIntro = await _appDbContext.WomenIntro.FindAsync(id);
            if (womenIntro == null) return NotFound();

            _fileService.Delete(womenIntro.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.WomenIntro.Remove(womenIntro);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var womenIntro = await _appDbContext.WomenIntro.FindAsync(id);
            if (womenIntro == null) return NotFound();

            var model = new WomenIntroDetailsVM
            {
                Id = womenIntro.Id,
                Title = womenIntro.Title,
                Description = womenIntro.Description,
                Url = womenIntro.Url,
                PhotoPath = womenIntro.PhotoPath
            };
            return View(model);
        }

        #endregion
    }
}
