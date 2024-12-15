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
            var newsList =  await _context.News
                .Include(n => n.Images)
                .Include(n => n.User)
                .OrderByDescending(n => n.NewsCreateAt)
                .ToListAsync();
            ViewBag.News = newsList;
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var newsDetails = await _context.News
                .Include(n => n.User)
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.NewsId == id);

            if (newsDetails == null)
            {
                return NotFound();
            }

            ViewBag.NewsDetails = newsDetails;

            ViewBag.Images = newsDetails.Images;

            ViewBag.UserFullName = newsDetails.User?.UserFullName;

            return View();
        }

        public IActionResult AddNews()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Access");
            }


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
                    NewsCreateAt = DateTime.Now,
                    UserId = userId,
                };

                await _context.News.AddAsync(news);
                await _context.SaveChangesAsync();

                var newsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "NewImage", news.NewsId.ToString());
                if (!Directory.Exists(newsDirectory))
                {
                    Directory.CreateDirectory(newsDirectory);
                }

                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    foreach (var file in model.ImageFiles)
                    {
                        var filePath = Path.Combine(newsDirectory, file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            NewsId = news.NewsId,
                            ImagePath = $"/NewImage/{news.NewsId}/{file.FileName}",
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

        public async Task<IActionResult> EditNews(int id)
        {
            var newsDetails = await _context.News
                .Include(n => n.User)
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.NewsId == id);

            if (newsDetails == null)
            {
                return NotFound();
            }

            ViewBag.NewsDetails = newsDetails;

            ViewBag.Images = newsDetails.Images;

            ViewBag.UserFullName = newsDetails.User?.UserFullName;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditNews(int id, NewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var news = await _context.News
                    .Include(n => n.Images)
                    .FirstOrDefaultAsync(n => n.NewsId == id);

                if (news == null)
                {
                    return NotFound();
                }

                news.NewsTitle = model.NewsTitle;
                news.NewsContent = model.NewsContent;
                news.NewsShortDescription = model.NewsShortDescription;
                news.NewsCreateAt = DateTime.Now;

                var newsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "NewImage", news.NewsId.ToString());

                if (!Directory.Exists(newsDirectory))
                {
                    Directory.CreateDirectory(newsDirectory);
                }

                if (model.ImageFiles != null && model.ImageFiles.Any())
                {
                    // Xóa ảnh cũ khỏi thư mục
                    foreach (var oldImage in news.Images)
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    _context.Images.RemoveRange(news.Images);

                    foreach (var file in model.ImageFiles)
                    {
                        var filePath = Path.Combine(newsDirectory, file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            NewsId = news.NewsId,
                            ImagePath = $"/NewImage/{news.NewsId}/{file.FileName}",
                            ImageCreateAt = DateTime.Now,
                        };

                        _context.Images.Add(image);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var newsDetails = await _context.News
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.NewsId == id);

            ViewBag.NewsDetails = newsDetails;
            ViewBag.Images = newsDetails.Images;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var news = await _context.News
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.NewsId == id);

            if (news == null)
            {
                return NotFound();
            }

            var newsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "NewImage", news.NewsId.ToString());
            if (Directory.Exists(newsDirectory))
            {
                foreach (var image in news.Images)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                Directory.Delete(newsDirectory, true);
            }

            _context.Images.RemoveRange(news.Images);

            _context.News.Remove(news);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
