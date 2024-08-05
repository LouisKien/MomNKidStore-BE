using System;
using System.Collections.Generic;

namespace MomNKidStore_Repository.Entities;

public partial class VoucherOfShop
{
    public int VoucherId { get; set; }

    public double VoucherValue { get; set; }

    public DateOnly StartDate { get; set; }

    public int VoucherQuantity { get; set; }

    public DateOnly EndDate { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
