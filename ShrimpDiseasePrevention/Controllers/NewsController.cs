using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShrimpDiseasePrevention.Models;
using ShrimpDiseasePrevention.ViewModels;
using System.Security.Claims;

namespace ShrimpDiseasePrevention.Controllers
{
    public class NewsController : Controller
    {
        private readonly ShrimpGuardContext _context;

        public NewsController(ShrimpGuardContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listNews = await _context.News.Include(n => n.User).ToListAsync();
            ViewBag.News = listNews;
            return View();
        }

        public IActionResult AddNews()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Access");
            }

            var userFullName = HttpContext.Session.GetString("FullName");

            ViewBag.UserId = userId.Value;
            ViewBag.UserFullName = userFullName;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddNews(NewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                var news = new News
                {
                    NewsTitle = model.NewsTitle,
                    NewsContent = model.NewsContent,
                    NewsShortDescription = model.NewsShortDescription,
                    NewsCreateAt = model.NewsCreateAt ?? DateTime.Now,
                    UserId = userId,
                };

                _context.News.Add(news);
                await _context.SaveChangesAsync();

                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    foreach (var file in model.ImageFiles)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "NewImage", file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            NewsId = news.NewsId,
                            ImagePath = $"/images/{file.FileName}",
                            ImageCreateAt = DateTime.Now,
                        };

                        _context.Images.Add(image);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

    }
}
