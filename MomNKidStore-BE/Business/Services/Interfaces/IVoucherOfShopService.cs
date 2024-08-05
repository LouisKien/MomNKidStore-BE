using MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IVoucherOfShopService
    {
        Task<List<VoucherOfShopDtoResponse>> Get();
        Task<VoucherOfShopDtoResponse?> Get(int id);
        Task<List<VoucherOfShopDtoResponseForAdmin>> GetByAdmin();
        Task<VoucherOfShopDtoResponseForAdmin?> GetByAdmin(int id);
        Task Post(VoucherOfShopDtoRequest request);
        Task<bool> Put(int id, VoucherOfShopDtoRequest request);
        Task<bool> UpdateStatus(int id, bool status);
    }
}
