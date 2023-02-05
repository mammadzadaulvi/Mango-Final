using Mango.Areas.Admin.ViewModels.MenPromote;
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
    public class MenPromoteController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenPromoteController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            var query = _appDbContext.MenPromotes.AsQueryable();
            var model = new MenPromoteIndexVM
            {
                MenPromotes = PaginatedList<MenPromote>.Create(query,5,page)
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
        public async Task<IActionResult> Create(MenPromoteCreateVM model)
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

            var menPromote = new MenPromote
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.MenPromotes.AddAsync(menPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion

        #region Update 


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var menPromote = await _appDbContext.MenPromotes.FindAsync(id);

            if (menPromote == null) return NotFound();

            var model = new MenPromoteUpdateVM
            {
                Id = menPromote.Id,
                Title = menPromote.Title,
                Url = menPromote.Url,
                PhotoPath = menPromote.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(MenPromoteUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var menPromote = await _appDbContext.MenPromotes.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (menPromote == null) return NotFound();

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

                _fileService.Delete(menPromote.PhotoPath, _webHostEnvironment.WebRootPath);
                menPromote.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            menPromote.Title = model.Title;
            menPromote.Url = model.Url;
            model.PhotoPath = menPromote.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var menPromote = await _appDbContext.MenPromotes.FindAsync(id);
            if (menPromote == null) return NotFound();

            _fileService.Delete(menPromote.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.MenPromotes.Remove(menPromote);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var menPromote = await _appDbContext.MenPromotes.FindAsync(id);
            if (menPromote == null) return NotFound();

            var model = new MenPromoteDetailsVM
            {
                Id = menPromote.Id,
                Title = menPromote.Title,
                Url = menPromote.Url,
                PhotoPath = menPromote.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
