using Mango.Areas.Admin.ViewModels.TeenIntro;
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
    public class TeenIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeenIntroController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new TeenIntroIndexVm
            {
                TeenIntro = await _appDbContext.TeenIntro.FirstOrDefaultAsync()
            };
            return View(model);
        }


        #region Create 

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var teenIntro = await _appDbContext.TeenIntro.FirstOrDefaultAsync();
            if (teenIntro != null) return NotFound();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(TeenIntroCreateVm model)
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

            var teenIntro = new TeenIntro
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.TeenIntro.AddAsync(teenIntro);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var teenIntro = await _appDbContext.TeenIntro.FindAsync(id);

            if (teenIntro == null) return NotFound();

            var model = new TeenIntroUpdateVm
            {
                Id = teenIntro.Id,
                Title = teenIntro.Title,
                Description = teenIntro.Description,
                Url = teenIntro.Url,
                PhotoPath = teenIntro.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(TeenIntroUpdateVm model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var teenIntro = await _appDbContext.TeenIntro.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (teenIntro == null) return NotFound();

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

                _fileService.Delete(teenIntro.PhotoPath, _webHostEnvironment.WebRootPath);
                teenIntro.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            teenIntro.Title = model.Title;
            teenIntro.Description = model.Description;
            teenIntro.Url = model.Url;
            model.PhotoPath = teenIntro.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var teenIntro = await _appDbContext.TeenIntro.FindAsync(id);
            if (teenIntro == null) return NotFound();

            _fileService.Delete(teenIntro.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.TeenIntro.Remove(teenIntro);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var teenIntro = await _appDbContext.TeenIntro.FindAsync(id);
            if (teenIntro == null) return NotFound();

            var model = new TeenIntroDetailsVm
            {
                Id = teenIntro.Id,
                Title = teenIntro.Title,
                Description = teenIntro.Description,
                Url = teenIntro.Url,
                PhotoPath = teenIntro.PhotoPath
            };
            return View(model);
        }

        #endregion
    }
}
