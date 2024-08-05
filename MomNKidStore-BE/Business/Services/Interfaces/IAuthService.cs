using MomNKidStore_BE.Business.ModelViews.AccountDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserAuthenticatingDtoResponse?> AuthenticateUser(UserAuthenticatingDtoRequest loginInfo);

        Task<string> GenerateAccessToken(UserAuthenticatingDtoResponse account);

        Task<bool> GetAccountByEmail(string email);

        Task<bool> CreateAccountCustomer(UserRegisterDtoRequest request);
    }
}
