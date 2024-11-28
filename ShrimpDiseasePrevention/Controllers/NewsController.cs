using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShrimpDiseasePrevention.Models;

namespace ShrimpDiseasePrevention.Controllers
{
    public class NewsController : Controller
    {
        private readonly ShrimpGuardContext _context;

        public NewsController(ShrimpGuardContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var list = _context.News.Include(n => n.User).ToList();
            return View(list);
        }
    }
}
