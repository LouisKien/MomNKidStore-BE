using System;
using System.Collections.Generic;

namespace MomNKidStore_Repository.Entities;

public partial class ProductCategory
{
    public int ProductCategoryId { get; set; }

    public string ProductCategoryName { get; set; } = null!;

    public bool ProductCategoryStatus { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
