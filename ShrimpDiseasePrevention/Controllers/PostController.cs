using Microsoft.AspNetCore.Mvc;
using ShrimpDiseasePrevention.Models;

namespace ShrimpDiseasePrevention.Controllers
{
    public class PostController : Controller
    {
        private readonly ShrimpGuardContext _context;

        public PostController(ShrimpGuardContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var listPost = _context.Posts.ToList();
            return View(listPost);
        }
    }
}
