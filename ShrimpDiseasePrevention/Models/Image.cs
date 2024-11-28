using System;
using System.Collections.Generic;

namespace ShrimpDiseasePrevention.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public int? NewsId { get; set; }

    public int? DiseaseId { get; set; }

    public int? PreventionId { get; set; }

    public int? PostId { get; set; }

    public string ImagePath { get; set; } = null!;

    public DateTime? ImageCreateAt { get; set; }

    public virtual Disease? Disease { get; set; }

    public virtual News? News { get; set; }

    public virtual Post? Post { get; set; }

    public virtual Prevention? Prevention { get; set; }
}
