using Microsoft.EntityFrameworkCore.Storage;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.Repositories.Implements;
using MomNKidStore_Repository.Repositories.Interfaces;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomNKidStore_Repository.UnitOfWorks.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private MomNkidStoreContext _context;
        private GenericRepository<Account> _accountRepository;
        private GenericRepository<Blog> _blogRepository;
        private GenericRepository<Cart> _cartRepository;
        private GenericRepository<Feedback> _feedbackRepository;
        private GenericRepository<Order> _orderRepository;
        private GenericRepository<OrderDetail> _orderDetailRepository;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<ProductCategory> _productCategoryRepository;
        private GenericRepository<ImageProduct> _imageProductRepository;
        private GenericRepository<Payment> _paymentRepository;
        private GenericRepository<Customer> _customerRepository;
        private GenericRepository<BlogProduct> _blogProductRepository;
        private GenericRepository<VoucherOfShop> _voucherOfShopRepository;
        public UnitOfWork(MomNkidStoreContext context)
        {
            _context = context;
        }

        public IGenericRepository<Account> AccountRepository => _accountRepository ??= new GenericRepository<Account>(_context);

        public IGenericRepository<Blog> BlogRepository => _blogRepository ??= new GenericRepository<Blog>(_context);

        public IGenericRepository<Cart> CartRepository => _cartRepository ??= new GenericRepository<Cart>(_context);

        public IGenericRepository<Feedback> FeedbackRepository => _feedbackRepository ??= new GenericRepository<Feedback>(_context);

        public IGenericRepository<Order> OrderRepository => _orderRepository ??= new GenericRepository<Order>(_context);

        public IGenericRepository<OrderDetail> OrderDetailRepository => _orderDetailRepository ??= new GenericRepository<OrderDetail>(_context);

        public IGenericRepository<Product> ProductRepository => _productRepository ??= new GenericRepository<Product>(_context);

        public IGenericRepository<ProductCategory> ProductCategoryRepository => _productCategoryRepository ??= new GenericRepository<ProductCategory>(_context);

        public IGenericRepository<ImageProduct> ImageProductRepository => _imageProductRepository ??= new GenericRepository<ImageProduct>(_context);

        public IGenericRepository<Payment> PaymentRepository => _paymentRepository ??= new GenericRepository<Payment>(_context);

        public IGenericRepository<Customer> CustomerRepository => _customerRepository ??= new GenericRepository<Customer>(_context);

        public IGenericRepository<BlogProduct> BlogProductRepository => _blogProductRepository ??= new GenericRepository<BlogProduct>(_context);

        public IGenericRepository<VoucherOfShop> VoucherOfShopRepository => _voucherOfShopRepository ??= new GenericRepository<VoucherOfShop>(_context);

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
