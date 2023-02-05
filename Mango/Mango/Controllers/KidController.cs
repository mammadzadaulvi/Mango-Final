using Mango.DAL;
using Mango.ViewModels.Kid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Controllers
{
    public class KidController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public KidController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new KidIndexVM
            {
                KidIntro = await _appDbContext.KidIntro.FirstOrDefaultAsync(),
                KidOtherLines = await _appDbContext.KidOtherLines.ToListAsync(),
                KidPromotes = await _appDbContext.KidPromotes.ToListAsync(),
            };


            return View(model);
        }
    }
}
