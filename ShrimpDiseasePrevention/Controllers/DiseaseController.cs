using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShrimpDiseasePrevention.Models;
using ShrimpDiseasePrevention.ViewModels;

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

            ViewBag.Diseases = diseaseList;
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var diseaseDetails = await _context.Diseases
                .Include(d => d.Preventions)
                .Include(d => d.Images)
                .Include(d => d.User)
                .FirstOrDefaultAsync(n => n.DiseaseId == id);

            var preventionDetails = await _context.Preventions
                .Include(p => p.Images)
                .FirstOrDefaultAsync(n => n.PreventionId == id);

            if (diseaseDetails == null)
            {
                return NotFound();
            }

            ViewBag.DiseaseDetails = diseaseDetails;
            ViewBag.Images = diseaseDetails.Images?.ToList() ?? new List<Image>();
            ViewBag.UserFullName = diseaseDetails.User?.UserFullName;
            ViewBag.PreventionDetails = preventionDetails;
            ViewBag.PreventionImages = preventionDetails?.Images?.ToList() ?? new List<Image>();

            return View();
        }

        public IActionResult AddDisease()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Access");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDisease(DiseaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                var disease = new Disease
                {
                    DiseaseTitle = model.DiseaseTitle,
                    DiseaseContent = model.DiseaseContent,
                    DiseaseShortDescription = model.DiseaseShortDescription,
                    DiseaseSymptoms = model.DiseaseSymptoms,
                    DiseaseCreateAt = DateTime.Now,
                    UserId = userId,
                };

                await _context.Diseases.AddAsync(disease);
                await _context.SaveChangesAsync();

                var diseaseImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DiseaseImage", disease.DiseaseId.ToString());
                if (!Directory.Exists(diseaseImageDirectory))
                {
                    Directory.CreateDirectory(diseaseImageDirectory);
                }

                if (model.DiseaseImageFiles != null && model.DiseaseImageFiles.Any())
                {
                    foreach (var file in model.DiseaseImageFiles)
                    {
                        var fileName = $"{disease.DiseaseId}_{file.FileName}";
                        var filePath = Path.Combine(diseaseImageDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            DiseaseId = disease.DiseaseId,
                            ImagePath = $"/DiseaseImage/{disease.DiseaseId}/{fileName}",
                            ImageCreateAt = DateTime.Now,
                        };

                        await _context.Images.AddAsync(image);
                    }
                }

                var prevention = new Prevention
                {
                    PreventionContent = model.PreventionContent,
                };

                await _context.Preventions.AddAsync(prevention);
                await _context.SaveChangesAsync();

                disease.Preventions.Add(prevention);

                var preventionImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PreventionImage", disease.DiseaseId.ToString());
                if (!Directory.Exists(preventionImageDirectory))
                {
                    Directory.CreateDirectory(preventionImageDirectory);
                }

                if (model.PreventionImageFiles != null && model.PreventionImageFiles.Any())
                {
                    foreach (var file in model.PreventionImageFiles)
                    {
                        var fileName = $"{disease.DiseaseId}_{file.FileName}";
                        var filePath = Path.Combine(preventionImageDirectory, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            PreventionId = prevention.PreventionId,
                            ImagePath = $"/PreventionImage/{disease.DiseaseId}/{fileName}",
                            ImageCreateAt = DateTime.Now,
                        };

                        await _context.Images.AddAsync(image);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> EditDisease(int id)
        {
            var diseaseDetails = await _context.Diseases
                .Include(d => d.Preventions)
                .Include(d => d.Images)
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.DiseaseId == id);

            var preventionDetails = await _context.Preventions
               .Include(p => p.Images)
               .FirstOrDefaultAsync(n => n.PreventionId == id);

            if (diseaseDetails == null)
            {
                return NotFound();
            }

            ViewBag.DiseaseDetails = diseaseDetails;
            ViewBag.Images = diseaseDetails.Images?.ToList() ?? new List<Image>();
            ViewBag.UserFullName = diseaseDetails.User?.UserFullName;
            ViewBag.PreventionDetails = preventionDetails;
            ViewBag.PreventionImages = preventionDetails?.Images?.ToList() ?? new List<Image>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditDisease(int id, DiseaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var disease = await _context.Diseases
                    .Include(d => d.Images) // Bao gồm ảnh liên quan
                    .Include(d => d.Preventions)
                    .FirstOrDefaultAsync(d => d.DiseaseId == id);

                if (disease == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin bệnh
                disease.DiseaseTitle = model.DiseaseTitle;
                disease.DiseaseContent = model.DiseaseContent;
                disease.DiseaseShortDescription = model.DiseaseShortDescription;
                disease.DiseaseSymptoms = model.DiseaseSymptoms;

                // Xử lý ảnh bệnh
                var diseaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DiseaseImage", disease.DiseaseId.ToString());
                if (!Directory.Exists(diseaseDirectory))
                {
                    Directory.CreateDirectory(diseaseDirectory);
                }

                if (model.DiseaseImageFiles != null && model.DiseaseImageFiles.Any())
                {
                    // Xóa ảnh cũ khỏi thư mục và cơ sở dữ liệu
                    foreach (var oldImage in disease.Images)
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    _context.Images.RemoveRange(disease.Images);

                    // Thêm ảnh mới
                    foreach (var file in model.DiseaseImageFiles)
                    {
                        var filePath = Path.Combine(diseaseDirectory, file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            DiseaseId = disease.DiseaseId,
                            ImagePath = $"/DiseaseImage/{disease.DiseaseId}/{file.FileName}",
                            ImageCreateAt = DateTime.Now,
                        };

                        _context.Images.Add(image);
                    }
                }

                // Xử lý thông tin phòng chống
                var prevention = disease.Preventions.FirstOrDefault();
                if (prevention != null)
                {
                    prevention.PreventionContent = model.PreventionContent;
                }

                var preventionDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PreventionImage", disease.DiseaseId.ToString());
                if (!Directory.Exists(preventionDirectory))
                {
                    Directory.CreateDirectory(preventionDirectory);
                }

                if (model.PreventionImageFiles != null && model.PreventionImageFiles.Any())
                {
                    // Xóa ảnh cũ của phòng chống
                    foreach (var oldImage in prevention?.Images ?? new List<Image>())
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    if (prevention != null)
                    {
                        _context.Images.RemoveRange(prevention.Images);
                    }

                    // Thêm ảnh mới cho phòng chống
                    foreach (var file in model.PreventionImageFiles)
                    {
                        var filePath = Path.Combine(preventionDirectory, file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new Image
                        {
                            PreventionId = prevention?.PreventionId,
                            ImagePath = $"/PreventionImage/{disease.DiseaseId}/{file.FileName}",
                            ImageCreateAt = DateTime.Now,
                        };

                        _context.Images.Add(image);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var diseaseDetails = await _context.Diseases
                .Include(d => d.Images)
                .Include(d => d.Preventions)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(d => d.DiseaseId == id);

            ViewBag.DiseaseDetails = diseaseDetails;
            ViewBag.Images = diseaseDetails?.Images;
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> DeleteDisease(int id)
        {
            var disease = await _context.Diseases
                .Include(d => d.Images) // Bao gồm ảnh liên quan
                .Include(d => d.Preventions)
                .ThenInclude(p => p.Images) // Bao gồm ảnh của phòng chống (nếu có)
                .FirstOrDefaultAsync(d => d.DiseaseId == id);

            if (disease == null)
            {
                return NotFound();
            }

            // Xử lý thư mục ảnh của bệnh
            var diseaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "DiseaseImage", disease.DiseaseId.ToString());
            if (Directory.Exists(diseaseDirectory))
            {
                foreach (var image in disease.Images)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                Directory.Delete(diseaseDirectory, true);
            }

            // Xử lý thư mục ảnh của phòng chống (nếu có)
            var preventionDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PreventionImage", disease.DiseaseId.ToString());
            if (Directory.Exists(preventionDirectory) && disease.Preventions.Any())
            {
                foreach (var prevention in disease.Preventions)
                {
                    foreach (var image in prevention.Images)
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                }
                Directory.Delete(preventionDirectory, true);
            }

            _context.Images.RemoveRange(disease.Images);
            foreach (var prevention in disease.Preventions)
            {
                if (prevention.Images != null)
                {
                    _context.Images.RemoveRange(prevention.Images);
                }
            }
            _context.Preventions.RemoveRange(disease.Preventions);
            _context.Diseases.Remove(disease);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}
