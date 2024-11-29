namespace ShrimpDiseasePrevention.ViewModels
{
    public class NewsViewModel
    {
        public int NewsId { get; set; }
        public string NewsTitle { get; set; } = null!;
        public string NewsContent { get; set; } = null!;
        public string? NewsShortDescription { get; set; }
        public DateTime? NewsCreateAt { get; set; }
        public int? UserId { get; internal set; }
        public string? UserFullName { get; set; }

        public IEnumerable<IFormFile>? ImageFiles { get; set; }
    }
}
