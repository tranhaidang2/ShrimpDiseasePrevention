using System;
using System.Collections.Generic;

namespace ShrimpDiseasePrevention.Models;

public partial class News
{
    public int NewsId { get; set; }

    public string NewsTitle { get; set; } = null!;

    public string NewsContent { get; set; } = null!;

    public string? NewsShortDescription { get; set; }

    public DateTime? NewsCreateAt { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual User? User { get; set; }
}
