using System;
using System.Collections.Generic;

namespace MomNKidStore_Repository.Entities;

public partial class Blog
{
    public int BlogId { get; set; }

    public string BlogTitle { get; set; } = null!;

    public string BlogContent { get; set; } = null!;

    public string? BlogImage { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<BlogProduct> BlogProducts { get; set; } = new List<BlogProduct>();
}
