using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_BE.Business.ModelViews.OrderDetailDTOs;
using MomNKidStore_BE.Business.ModelViews.OrderDTOs;
using MomNKidStore_BE.Business.ModelViews.PaymentDTOs;
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

            CreateMap<ProductCategory, CategoryDto>().ReverseMap();
            CreateMap<Product, ProductDtoResponse>().ReverseMap();
            CreateMap<Product, ProductDtoRequest>().ReverseMap();

            CreateMap<VoucherOfShop, VoucherOfShopDtoRequest>().ReverseMap();
            CreateMap<VoucherOfShop, VoucherOfShopDtoResponse>().ReverseMap();
            CreateMap<VoucherOfShop, VoucherOfShopDtoResponseForAdmin>().ReverseMap();

            CreateMap<Payment, PaymentDtoResponse>().ReverseMap();

            CreateMap<Order, OrderDtoResponse>().ReverseMap();

            CreateMap<OrderDetail, OrderDetailDtoResponse>().ReverseMap();
        }
    }
}
