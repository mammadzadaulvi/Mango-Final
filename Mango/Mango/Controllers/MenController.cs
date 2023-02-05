using Mango.DAL;
using Mango.ViewModels.Men;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Controllers
{
    public class MenController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public MenController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new MenIndexVM
            {
                MenIntro = await _appDbContext.MenIntro.FirstOrDefaultAsync(),
                MenOtherLines = await _appDbContext.MenOtherLines.ToListAsync(),
                MenPromotes = await _appDbContext.MenPromotes.ToListAsync(),
            };


            return View(model);
        }
    }
}
