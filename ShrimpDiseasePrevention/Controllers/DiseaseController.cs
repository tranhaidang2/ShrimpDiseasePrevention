using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShrimpDiseasePrevention.Models;

namespace ShrimpDiseasePrevention.Controllers
{
    public class DiseaseController : Controller
    {
        private readonly ShrimpGuardContext _context;

        public DiseaseController(ShrimpGuardContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var diseaseList = await _context.Diseases
                .Include(d => d.Preventions)
                .Include(d => d.Images)
                .Include(d => d.User)
                .ToListAsync();

            if (diseaseList == null)
            {
                diseaseList = new List<Disease>();
            }

            ViewBag.Diseases = diseaseList;
            return View();
        }
    }
}
