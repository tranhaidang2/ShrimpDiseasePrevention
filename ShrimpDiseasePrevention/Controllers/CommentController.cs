using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShrimpDiseasePrevention.Models;

namespace ShrimpDiseasePrevention.Controllers
{
    public class CommentController : Controller
    {
        private readonly ShrimpGuardContext _context;

        public CommentController(ShrimpGuardContext context)
        {
            _context = context;
        }

        
    }
}
