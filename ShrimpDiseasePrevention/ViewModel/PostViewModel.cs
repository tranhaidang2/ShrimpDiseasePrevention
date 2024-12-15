namespace ShrimpDiseasePrevention.ViewModel
{
    public class PostViewModel
    {
        public int PostId { get; set; }

        public string PostTitle { get; set; } = null!;

        public string PostContent { get; set; } = null!;

        public DateTime? PostCreateAt { get; set; }

        public int? UserId { get; set; }

        public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
    }
}
