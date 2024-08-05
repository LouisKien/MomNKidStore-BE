using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_Repository.Entities;

namespace MomNKidStore_BE.Business.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, UserAuthenticatingDtoResponse>().ReverseMap();
            CreateMap<Account, UserRegisterDtoRequest>().ReverseMap();
        }
    }
}
