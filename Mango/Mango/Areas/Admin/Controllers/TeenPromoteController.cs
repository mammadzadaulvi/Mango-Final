using Mango.Areas.Admin.ViewModels.TeenPromote;
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
    public class TeenPromoteController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeenPromoteController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new TeenPromoteIndexVM
            {
                TeenPromotes = await _appDbContext.TeenPromotes.ToListAsync()
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
        public async Task<IActionResult> Create(TeenPromoteCreateVM model)
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

            var teenPromote = new TeenPromote
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.TeenPromotes.AddAsync(teenPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update 


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var teenPromote = await _appDbContext.TeenPromotes.FindAsync(id);

            if (teenPromote == null) return NotFound();

            var model = new TeenPromoteUpdateVM
            {
                Id = teenPromote.Id,
                Title = teenPromote.Title,
                Url = teenPromote.Url,
                PhotoPath = teenPromote.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(TeenPromoteUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var teenPromote = await _appDbContext.TeenPromotes.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (teenPromote == null) return NotFound();

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

                _fileService.Delete(teenPromote.PhotoPath, _webHostEnvironment.WebRootPath);
                teenPromote.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            teenPromote.Title = model.Title;
            teenPromote.Url = model.Url;
            model.PhotoPath = teenPromote.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var teenPromote = await _appDbContext.TeenPromotes.FindAsync(id);
            if (teenPromote == null) return NotFound();

            _fileService.Delete(teenPromote.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.TeenPromotes.Remove(teenPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var teenPromote = await _appDbContext.TeenPromotes.FindAsync(id);
            if (teenPromote == null) return NotFound();

            var model = new TeenPromoteDetailsVM
            {
                Id = teenPromote.Id,
                Title = teenPromote.Title,
                Url = teenPromote.Url,
                PhotoPath = teenPromote.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
