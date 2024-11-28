using System;
using System.Collections.Generic;

namespace ShrimpDiseasePrevention.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string PostTitle { get; set; } = null!;

    public string PostContent { get; set; } = null!;

    public DateTime? PostCreateAt { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual User? User { get; set; }
}
