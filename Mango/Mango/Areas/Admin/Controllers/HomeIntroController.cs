using Mango.Areas.Admin.ViewModels.HomeIntro;
using Mango.DAL;
using Mango.Helpers;
using Mango.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class HomeIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeIntroController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {

            var model = new HomeIntroIndexVM
            {
                HomeIntro = await _appDbContext.HomeIntro.FirstOrDefaultAsync()
            };
            return View(model);
        }


        #region Create 


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var homeIntro = await _appDbContext.HomeIntro.FirstOrDefaultAsync();
            if (homeIntro != null) return NotFound();
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(HomeIntroCreateVM model)
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

            var homeIntro = new HomeIntro
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.HomeIntro.AddAsync(homeIntro);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        #endregion


        #region Update

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var homeIntro = await _appDbContext.HomeIntro.FindAsync(id);

            if (homeIntro == null) return NotFound();


            var model = new HomeIntroUpdateVM
            {
                Id = homeIntro.Id,
                Title = homeIntro.Title,
                Description = homeIntro.Description,
                Url = homeIntro.Url,
                PhotoPath = homeIntro.PhotoPath
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Update(HomeIntroUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var HomeIntro = await _appDbContext.HomeIntro.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (HomeIntro == null) return NotFound();

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

                _fileService.Delete(HomeIntro.PhotoPath, _webHostEnvironment.WebRootPath);
                HomeIntro.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            HomeIntro.Title = model.Title;
            HomeIntro.Description = model.Description;
            HomeIntro.Url = model.Url;
            model.PhotoPath = HomeIntro.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var homeIntro = await _appDbContext.HomeIntro.FindAsync(id);
            if (homeIntro == null) return NotFound();

            _fileService.Delete(homeIntro.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.HomeIntro.Remove(homeIntro);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var homeIntro = await _appDbContext.HomeIntro.FindAsync(id);
            if (homeIntro == null) return NotFound();

            var model = new HomeIntroDetailsVM
            {
                Id = homeIntro.Id,
                Title = homeIntro.Title,
                Description = homeIntro.Description,
                Url = homeIntro.Url,
                PhotoPath = homeIntro.PhotoPath
            };
            return View(model);
        }

        #endregion
    }
}
