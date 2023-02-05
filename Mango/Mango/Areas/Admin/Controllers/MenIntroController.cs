using Mango.Areas.Admin.ViewModels.MenIntro;
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
    public class MenIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenIntroController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new MenIntroIndexVM
            {
                MenIntro = await _appDbContext.MenIntro.FirstOrDefaultAsync()
            };
            return View(model);
        }


        #region Create 

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var menIntro = await _appDbContext.MenIntro.FirstOrDefaultAsync();
            if (menIntro != null) return NotFound();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(MenIntroCreateVM model)
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

            var menIntro = new MenIntro
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.MenIntro.AddAsync(menIntro);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var menIntro = await _appDbContext.MenIntro.FindAsync(id);

            if (menIntro == null) return NotFound();

            var model = new MenIntroUpdateVM
            {
                Id = menIntro.Id,
                Title = menIntro.Title,
                Description = menIntro.Description,
                Url = menIntro.Url,
                PhotoPath = menIntro.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(MenIntroUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var menIntro = await _appDbContext.MenIntro.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (menIntro == null) return NotFound();

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

                _fileService.Delete(menIntro.PhotoPath, _webHostEnvironment.WebRootPath);
                menIntro.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            menIntro.Title = model.Title;
            menIntro.Description = model.Description;
            menIntro.Url = model.Url;
            model.PhotoPath = menIntro.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var menIntro = await _appDbContext.MenIntro.FindAsync(id);
            if (menIntro == null) return NotFound();

            _fileService.Delete(menIntro.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.MenIntro.Remove(menIntro);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var menIntro = await _appDbContext.MenIntro.FindAsync(id);
            if (menIntro == null) return NotFound();

            var model = new MenIntroDetailsVM
            {
                Id = menIntro.Id,
                Title = menIntro.Title,
                Description = menIntro.Description,
                Url = menIntro.Url,
                PhotoPath = menIntro.PhotoPath
            };
            return View(model);
        }

        #endregion
    }
}
