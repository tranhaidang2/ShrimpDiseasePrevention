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

            if (diseaseDetails == null)
            {
                return NotFound();
            }

            ViewBag.DiseaseDetails = diseaseDetails;

            ViewBag.Images = diseaseDetails.Images;

            ViewBag.UserFullName = diseaseDetails.User?.UserFullName;

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

                        _context.Images.Add(image);
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

    }
}
