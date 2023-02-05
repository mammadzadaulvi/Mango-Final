using Mango.DAL;
using Mango.ViewModels.Women;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Controllers
{
    public class WomenController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public WomenController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<IActionResult> Index()
        {
            var model = new WomenIndexVM
            {
                WomenIntro = await _appDbContext.WomenIntro.FirstOrDefaultAsync(),
                WomenMangoGirls = await _appDbContext.WomenMangoGirls
                                                     .OrderByDescending(wmg => wmg.Id)
                                                     .Take(4)
                                                     .ToListAsync(),
                WomenOtherLines = await _appDbContext.WomenOtherLines.ToListAsync(),
                WomenPromotes = await _appDbContext.WomenPromotes.ToListAsync(),
            };


            return View(model);
        }

        public async Task<IActionResult> LoadMore(int skipRow)
        {
            bool isLast = false;
            var womenMangoGirl = await _appDbContext.WomenMangoGirls
                               .OrderByDescending(wmg => wmg.Id)
                               .Skip(4 * skipRow)
                               .Take(4)
                               .ToListAsync();

            if ((4 * skipRow) + 4 > _appDbContext.WomenMangoGirls.Count())
            {
                isLast = true;
            }

            var model = new MangoGirlLoadMoreVM
            {
                WomenMangoGirls = womenMangoGirl,
                IsLast = isLast
            };

            return PartialView("_MangoGirlPartial", model);
        }
    }
}
