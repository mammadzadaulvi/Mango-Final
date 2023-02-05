using Mango.DAL;
using Mango.ViewModels.Teen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Controllers
{
    public class TeenController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public TeenController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var model = new TeenIndexVM
            {
                TeenIntro = await _appDbContext.TeenIntro.FirstOrDefaultAsync(),
                TeenOtherLines = await _appDbContext.TeenOtherLines.ToListAsync(),
                TeenPromotes = await _appDbContext.TeenPromotes.ToListAsync(),
            };


            return View(model);
        }
    }
}
