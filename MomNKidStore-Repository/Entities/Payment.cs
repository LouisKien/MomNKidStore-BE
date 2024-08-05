using System;
using System.Collections.Generic;

namespace MomNKidStore_Repository.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? BankCode { get; set; }

    public string? BankTranNo { get; set; }

    public string? CardType { get; set; }

    public string? PaymentInfo { get; set; }

    public DateTime? PayDate { get; set; }

    public string? TransactionNo { get; set; }

    public int? TransactionStatus { get; set; }

    public decimal? PaymentAmount { get; set; }

    public virtual Order Order { get; set; } = null!;
}
