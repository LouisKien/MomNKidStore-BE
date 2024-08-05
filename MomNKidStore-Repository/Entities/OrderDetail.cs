using System;
using System.Collections.Generic;

namespace MomNKidStore_Repository.Entities;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int OrderQuantity { get; set; }

    public double ProductPrice { get; set; }

    public bool Status { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
