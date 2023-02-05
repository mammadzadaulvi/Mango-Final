using Mango.Areas.Admin.ViewModels.HomePromote;
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
    public class HomePromoteController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomePromoteController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _appDbContext.HomePromotes.AsQueryable();
            var model = new HomePromoteIndexVM
            {
                HomePromotes = PaginatedList<HomePromote>.Create(query,5,page)
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
        public async Task<IActionResult> Create(HomePromoteCreateVM model)
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

            var homePromote = new HomePromote
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.HomePromotes.AddAsync(homePromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update 


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var homePromote = await _appDbContext.HomePromotes.FindAsync(id);

            if (homePromote == null) return NotFound();

            var model = new HomePromoteUpdateVM
            {
                Id = homePromote.Id,
                Title = homePromote.Title,
                Url = homePromote.Url,
                PhotoPath = homePromote.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(HomePromoteUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var homePromote = await _appDbContext.HomePromotes.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (homePromote == null) return NotFound();

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

                _fileService.Delete(homePromote.PhotoPath, _webHostEnvironment.WebRootPath);
                homePromote.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            homePromote.Title = model.Title;
            homePromote.Url = model.Url;
            model.PhotoPath = homePromote.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var homePromote = await _appDbContext.HomePromotes.FindAsync(id);
            if (homePromote == null) return NotFound();

            _fileService.Delete(homePromote.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.HomePromotes.Remove(homePromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var homePromote = await _appDbContext.HomePromotes.FindAsync(id);
            if (homePromote == null) return NotFound();

            var model = new HomePromoteDetailsVM
            {
                Id = homePromote.Id,
                Title = homePromote.Title,
                Url = homePromote.Url,
                PhotoPath = homePromote.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
