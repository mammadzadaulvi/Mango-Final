using Mango.Areas.Admin.ViewModels.KidIntro;
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
    public class KidIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public KidIntroController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {

            var model = new KidIntroIndexVm
            {
                KidIntro = await _appDbContext.KidIntro.FirstOrDefaultAsync()
            };
            return View(model);
        }


        #region Create 

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var kidIntro = await _appDbContext.KidIntro.FirstOrDefaultAsync();
            if (kidIntro != null) return NotFound();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(KidIntroCreateVm model)
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

            var kidIntro = new KidIntro
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.KidIntro.AddAsync(kidIntro);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var kidIntro = await _appDbContext.KidIntro.FindAsync(id);

            if (kidIntro == null) return NotFound();

            var model = new KidIntroUpdateVm
            {
                Id = kidIntro.Id,
                Title = kidIntro.Title,
                Description = kidIntro.Description,
                Url = kidIntro.Url,
                PhotoPath = kidIntro.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(KidIntroUpdateVm model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var KidIntro = await _appDbContext.KidIntro.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (KidIntro == null) return NotFound();

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

                _fileService.Delete(KidIntro.PhotoPath, _webHostEnvironment.WebRootPath);
                KidIntro.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            KidIntro.Title = model.Title;
            KidIntro.Description = model.Description;
            KidIntro.Url = model.Url;
            model.PhotoPath = KidIntro.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var kidIntro = await _appDbContext.KidIntro.FindAsync(id);
            if (kidIntro == null) return NotFound();

            _fileService.Delete(kidIntro.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.KidIntro.Remove(kidIntro);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var kidIntro = await _appDbContext.KidIntro.FindAsync(id);
            if (kidIntro == null) return NotFound();

            var model = new KidIntroDetailsVm
            {
                Id = kidIntro.Id,
                Title = kidIntro.Title,
                Description = kidIntro.Description,
                Url = kidIntro.Url,
                PhotoPath = kidIntro.PhotoPath
            };
            return View(model);
        }

        #endregion
    }
}
