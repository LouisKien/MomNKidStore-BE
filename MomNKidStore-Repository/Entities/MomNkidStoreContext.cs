using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MomNKidStore_Repository.Entities;

public partial class MomNkidStoreContext : DbContext
{
    public MomNkidStoreContext()
    {
    }

    public MomNkidStoreContext(DbContextOptions<MomNkidStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BlogProduct> BlogProducts { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<ImageProduct> ImageProducts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<VoucherOfShop> VoucherOfShops { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__F267251E30B7D1F2");

            entity.ToTable("Account");

            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Email).HasMaxLength(64);
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__FA0AA72D615CD55D");

            entity.ToTable("Blog");

            entity.Property(e => e.BlogId).HasColumnName("blogId");
            entity.Property(e => e.BlogContent)
                .HasMaxLength(350)
                .HasColumnName("blogContent");
            entity.Property(e => e.BlogImage).HasColumnName("blogImage");
            entity.Property(e => e.BlogTitle)
                .HasMaxLength(50)
                .HasColumnName("blogTitle");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<BlogProduct>(entity =>
        {
            entity.HasKey(e => e.BlogProductId).HasName("PK__BlogProd__21FA6EDB513AB591");

            entity.ToTable("BlogProduct");

            entity.Property(e => e.BlogProductId).HasColumnName("blogProductId");
            entity.Property(e => e.BlogId).HasColumnName("blogId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogProducts)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_blogId_BlogProduct");

            entity.HasOne(d => d.Product).WithMany(p => p.BlogProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_productId_BlogProduct");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Cart__415B03B81F7EDAFB");

            entity.ToTable("Cart");

            entity.Property(e => e.CartId).HasColumnName("cartId");
            entity.Property(e => e.CartQuantity).HasColumnName("cartQuantity");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerId_Cart");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_productId_Cart");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__B611CB7D11252FF0");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.AccountId, "UQ__Customer__F267251FDFB5F2CC").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.AccountId).HasColumnName("accountId");
            entity.Property(e => e.Address).HasMaxLength(30);
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Phone).HasMaxLength(10);
            entity.Property(e => e.Point).HasColumnName("point");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("userName");

            entity.HasOne(d => d.Account).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_accountId_Customer");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__2613FD2417226CF2");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.FeedbackContent)
                .HasMaxLength(250)
                .HasColumnName("feedbackContent");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.RateNumber).HasColumnName("rateNumber");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerId_Feedback");

            entity.HasOne(d => d.Product).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_productId_Feedback");
        });

        modelBuilder.Entity<ImageProduct>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__ImagePro__336E9B5551730F4B");

            entity.ToTable("ImageProduct");

            entity.Property(e => e.ImageId).HasColumnName("imageId");
            entity.Property(e => e.ImageProduct1).HasColumnName("imageProduct");
            entity.Property(e => e.ProductId).HasColumnName("productId");

            entity.HasOne(d => d.Product).WithMany(p => p.ImageProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_productId_ImageProduct");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__0809335D28B2A88A");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.CustomerId).HasColumnName("customerId");
            entity.Property(e => e.ExchangedPoint).HasColumnName("exchangedPoint");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("orderDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(13, 2)");
            entity.Property(e => e.VoucherId).HasColumnName("voucherId");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_customerId_Order");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK_voucherId_Order");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__E4FEDE4A788D6DFC");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("orderDetailId");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.OrderQuantity).HasColumnName("orderQuantity");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.ProductPrice).HasColumnName("productPrice");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orderId_OrderDetail");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_productId_OrderDetail");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A388E25C9A5");

            entity.ToTable("Payment");

            entity.HasIndex(e => e.OrderId, "UQ__Payment__0809335CD9F340E2").IsUnique();

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.PayDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentAmount).HasColumnType("decimal(13, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orderId_Payments");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__2D10D16A0224DD2D");

            entity.ToTable("Product", tb => tb.HasTrigger("trg_UpdateProductStatus"));

            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.ProductCategoryId).HasColumnName("productCategoryId");
            entity.Property(e => e.ProductInfor)
                .HasMaxLength(250)
                .HasColumnName("productInfor");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .HasColumnName("productName");
            entity.Property(e => e.ProductPrice)
                .HasColumnType("decimal(13, 2)")
                .HasColumnName("productPrice");
            entity.Property(e => e.ProductQuantity).HasColumnName("productQuantity");
            entity.Property(e => e.ProductStatus).HasColumnName("productStatus");

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_productCategoryId_Product");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId).HasName("PK__ProductC__A944253B7399188B");

            entity.ToTable("ProductCategory");

            entity.Property(e => e.ProductCategoryId).HasColumnName("productCategoryId");
            entity.Property(e => e.ProductCategoryName)
                .HasMaxLength(50)
                .HasColumnName("productCategoryName");
            entity.Property(e => e.ProductCategoryStatus).HasColumnName("productCategoryStatus");
        });

        modelBuilder.Entity<VoucherOfShop>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__VoucherO__F53389E975067780");

            entity.ToTable("VoucherOfShop");

            entity.Property(e => e.VoucherId).HasColumnName("voucherId");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.VoucherQuantity).HasColumnName("voucherQuantity");
            entity.Property(e => e.VoucherValue).HasColumnName("voucherValue");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
