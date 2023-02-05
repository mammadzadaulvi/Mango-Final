using Mango.Areas.Admin.ViewModels.TeenOtherLine;
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
    public class TeenOtherLineController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeenOtherLineController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new TeenOtherLineIndexVM
            {
                TeenOtherLines = await _appDbContext.TeenOtherLines.ToListAsync()
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
        public async Task<IActionResult> Create(TeenOtherLineCreateVM model)
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

            var teenOtherLine = new TeenOtherLine
            {
                Title = model.Title,
                Url = model.Url,
                PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.TeenOtherLines.AddAsync(teenOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Update 

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var teenOtherLine = await _appDbContext.TeenOtherLines.FindAsync(id);

            if (teenOtherLine == null) return NotFound();

            var model = new TeenOtherLineUpdateVM
            {
                Id = teenOtherLine.Id,
                Title = teenOtherLine.Title,
                Url = teenOtherLine.Url,
                PhotoPath = teenOtherLine.PhotoPath
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(TeenOtherLineUpdateVM model, int id)
        {
            if (!ModelState.IsValid) return View(model);

            var teenOtherLine = await _appDbContext.TeenOtherLines.FindAsync(id);

            if (id != model.Id) return BadRequest();

            if (teenOtherLine == null) return NotFound();

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

                _fileService.Delete(teenOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);
                teenOtherLine.PhotoPath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }

            teenOtherLine.Title = model.Title;
            teenOtherLine.Url = model.Url;
            model.PhotoPath = teenOtherLine.PhotoPath;

            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Delete 

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var teenOtherLine = await _appDbContext.TeenOtherLines.FindAsync(id);
            if (teenOtherLine == null) return NotFound();

            _fileService.Delete(teenOtherLine.PhotoPath, _webHostEnvironment.WebRootPath);

            _appDbContext.TeenOtherLines.Remove(teenOtherLine);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        #endregion


        #region Details 

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var teenOtherLine = await _appDbContext.TeenOtherLines.FindAsync(id);
            if (teenOtherLine == null) return NotFound();

            var model = new TeenOtherLineDetailsVM
            {
                Id = teenOtherLine.Id,
                Title = teenOtherLine.Title,
                Url = teenOtherLine.Url,
                PhotoPath = teenOtherLine.PhotoPath
            };
            return View(model);
        }
        #endregion
    }
}
