using MomNKidStore_BE.Business.ModelViews.AccountDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IAdminService
    {
        Task<bool> CreateAccountStaff(UserRegisterDtoRequest newAccount);
        Task<bool> LockAccount(int accountId);
        Task<bool> UnlockAccount(int accountId);
    }
}
