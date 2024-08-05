using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_BE.Business.ModelViews.CartDTOs;
using MomNKidStore_BE.Business.ModelViews.CustomerDTOs;
using MomNKidStore_BE.Business.ModelViews.OrderDetailDTOs;
using MomNKidStore_BE.Business.ModelViews.OrderDTOs;
using MomNKidStore_BE.Business.ModelViews.PaymentDTOs;
using MomNKidStore_BE.Business.ModelViews.ProductCategoryDTOs;
using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs;
using MomNKidStore_Repository.Entities;

namespace MomNKidStore_BE.Business.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, UserAuthenticatingDtoResponse>().ReverseMap();
            CreateMap<Account, UserRegisterDtoRequest>().ReverseMap();

            CreateMap<Cart, CartDtoRequest>().ReverseMap();
            CreateMap<Cart, CartDtoResponse>().ReverseMap();

            CreateMap<ProductCategory, CategoryDtoResponse>().ReverseMap();
            CreateMap<Product, ProductDtoResponse>().ReverseMap();
            CreateMap<Product, ProductDtoRequest>().ReverseMap();

            CreateMap<VoucherOfShop, VoucherOfShopDtoRequest>().ReverseMap();
            CreateMap<VoucherOfShop, VoucherOfShopDtoResponse>().ReverseMap();
            CreateMap<VoucherOfShop, VoucherOfShopDtoResponseForAdmin>().ReverseMap();

            CreateMap<Payment, PaymentDtoResponse>().ReverseMap();

            CreateMap<Order, OrderDtoResponse>().ReverseMap();

            CreateMap<OrderDetail, OrderDetailDtoResponse>().ReverseMap();

            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDto>().ReverseMap();

        }
    }
}
