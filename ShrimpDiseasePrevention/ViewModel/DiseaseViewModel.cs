namespace ShrimpDiseasePrevention.ViewModels
{
    public class DiseaseViewModel
    {
        public int DiseaseId { get; set; }

        public string DiseaseTitle { get; set; } = null!;

        public string DiseaseContent { get; set; } = null!;

        public string? DiseaseShortDescription { get; set; }

        public string? DiseaseSymptoms { get; set; }

        public DateTime? DiseaseCreateAt { get; set; }

        public int? UserId { get; internal set; }

        public string? UserFullName { get; set; }

        public string? PreventionContent { get; set; }

        public IEnumerable<IFormFile>? DiseaseImageFiles { get; set; }

        public IEnumerable<IFormFile>? PreventionImageFiles { get; set; }
    }
}
