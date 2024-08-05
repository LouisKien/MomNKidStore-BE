using System;
using System.Collections.Generic;

namespace MomNKidStore_Repository.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public int ProductCategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public string ProductInfor { get; set; } = null!;

    public decimal ProductPrice { get; set; }

    public int ProductQuantity { get; set; }

    public bool ProductStatus { get; set; }

    public virtual ICollection<BlogProduct> BlogProducts { get; set; } = new List<BlogProduct>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<ImageProduct> ImageProducts { get; set; } = new List<ImageProduct>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductCategory ProductCategory { get; set; } = null!;
}
