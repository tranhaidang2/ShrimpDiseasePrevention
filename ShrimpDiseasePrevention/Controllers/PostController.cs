using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShrimpDiseasePrevention.Models;
using ShrimpDiseasePrevention.ViewModel;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ShrimpDiseasePrevention.Controllers
{
    public class PostController : Controller
    {
        private readonly ShrimpGuardContext _context;

        public PostController(ShrimpGuardContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listPost = await _context.Posts
                .Include(p => p.Images)
                .Include(p => p.Comments)
                .Include(p => p.User)
                .ToListAsync();
            return View(listPost);
        }

        [HttpGet]
        public IActionResult AddPost()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Access");
            }

            return View(new PostViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(PostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var userId = HttpContext.Session.GetInt32("UserId");

            var post = new Post
            {
                PostTitle = model.PostTitle,
                PostContent = model.PostContent,
                PostCreateAt = DateTime.Now,
                UserId = userId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            var postDirectory = Path.Combine("wwwroot/PostImage", post.PostId.ToString());
            if (!Directory.Exists(postDirectory))
            {
                Directory.CreateDirectory(postDirectory);
            }

            if (model.Images != null && model.Images.Any())
            {
                foreach (var imageFile in model.Images)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(postDirectory, fileName);

                    // Lưu file vào thư mục
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    var image = new Image
                    {
                        ImagePath = $"/PostImage/{post.PostId}/{fileName}",
                        PostId = post.PostId,
                        ImageCreateAt = DateTime.Now
                    };

                    _context.Images.Add(image);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _context.Posts
                                     .Include(p => p.Comments)
                                     .Include(p => p.Images)
                                     .Include(p => p.User)
                                     .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            foreach (var comment in post.Comments)
            {
                _context.Comments.Remove(comment);
            }

            foreach (var image in post.Images)
            {
                var imagePath = Path.Combine("wwwroot", image.ImagePath.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _context.Images.Remove(image);
            }

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();

            var postDirectory = Path.Combine("wwwroot/PostImage", post.PostId.ToString());
            if (Directory.Exists(postDirectory))
            {
                Directory.Delete(postDirectory, true);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsPost(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .Include(p => p.User)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string commentContent)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Access");
            }

            var comment = new Comment
            {
                CommentContent = commentContent,
                CommentCreateAt = DateTime.Now,
                PostId = postId,
                UserId = userId.Value
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsPost", new { postId = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }

            //var currentUserId = HttpContext.Session.GetInt32("UserId");

            //if (currentUserId == null || comment.UserId != currentUserId)
            //{
            //    if (!User.IsInRole("Admin"))
            //    {
            //        return Forbid();
            //    }
            //}

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsPost", new { postId = comment.PostId });
        }

    }
}
