using Mango.Areas.Admin.ViewModels.KidPromote;
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
    public class KidPromoteController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public KidPromoteController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new KidPromoteIndexVM
            {
                KidPromotes = await _appDbContext.KidPromotes.ToListAsync()
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
        public async Task<IActionResult> Create(KidPromoteCreateVM model)
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

            var kidPromote = new KidPromote
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.KidPromotes.AddAsync(kidPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update 


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var kidPromote = await _appDbContext.KidPromotes.FindAsync(id);

            if (kidPromote == null) return NotFound();

            var model = new KidPromoteUpdateVM
            {
                Id = kidPromote.Id,
                Title = kidPromote.Title,
                Url = kidPromote.Url,
                PhotoPath = kidPromote.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(KidPromoteUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var kidPromote = await _appDbContext.KidPromotes.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (kidPromote == null) return NotFound();

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

                _fileService.Delete(kidPromote.PhotoPath, _webHostEnvironment.WebRootPath);
                kidPromote.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            kidPromote.Title = model.Title;
            kidPromote.Url = model.Url;
            model.PhotoPath = kidPromote.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var kidPromote = await _appDbContext.KidPromotes.FindAsync(id);
            if (kidPromote == null) return NotFound();

            _fileService.Delete(kidPromote.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.KidPromotes.Remove(kidPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var kidPromote = await _appDbContext.KidPromotes.FindAsync(id);
            if (kidPromote == null) return NotFound();

            var model = new KidPromoteDetailsVM
            {
                Id = kidPromote.Id,
                Title = kidPromote.Title,
                Url = kidPromote.Url,
                PhotoPath = kidPromote.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
