using System;
using System.Collections.Generic;

namespace MomNKidStore_Repository.Entities;

public partial class Cart
{
    public int CartId { get; set; }

    public int ProductId { get; set; }

    public int CustomerId { get; set; }

    public int CartQuantity { get; set; }

    public bool Status { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
