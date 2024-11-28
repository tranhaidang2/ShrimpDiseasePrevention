using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShrimpDiseasePrevention.Models;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string UserPassword { get; set; } = null!;

    [Required]
    public string? UserFullName { get; set; } = null!;

    public DateTime? UserCreateAt { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Disease> Diseases { get; set; } = new List<Disease>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual UserRole? Role { get; set; } = null!;
}
