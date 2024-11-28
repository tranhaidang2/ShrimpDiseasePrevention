using System;
using System.Collections.Generic;

namespace ShrimpDiseasePrevention.Models;

public partial class Disease
{
    public int DiseaseId { get; set; }

    public string DiseaseTitle { get; set; } = null!;

    public string DiseaseContent { get; set; } = null!;

    public string? DiseaseShortDescription { get; set; }

    public string? DiseaseSymptoms { get; set; }

    public DateTime? DiseaseCreateAt { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual User? User { get; set; }

    public virtual ICollection<Prevention> Preventions { get; set; } = new List<Prevention>();
}
