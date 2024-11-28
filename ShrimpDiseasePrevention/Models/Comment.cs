using System;
using System.Collections.Generic;

namespace ShrimpDiseasePrevention.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string CommentContent { get; set; } = null!;

    public DateTime? CommentCreateAt { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public int? ParentCommentId { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
