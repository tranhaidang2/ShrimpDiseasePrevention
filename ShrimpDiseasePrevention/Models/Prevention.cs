using System;
using System.Collections.Generic;

namespace ShrimpDiseasePrevention.Models;

public partial class Prevention
{
    public int PreventionId { get; set; }

    public string PreventionContent { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Disease> Diseases { get; set; } = new List<Disease>();
}
