using Mango.DAL;
using Mango.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexVM
            {
                HomeIntro = await _appDbContext.HomeIntro.FirstOrDefaultAsync(),
                HomeOtherLines = await _appDbContext.HomeOtherLines.ToListAsync(),
                HomePromotes = await _appDbContext.HomePromotes.ToListAsync(),
            };


            return View(model);
        }
    }
}
