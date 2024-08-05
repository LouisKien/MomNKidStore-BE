using MomNKidStore_BE.Business.ModelViews.CustomerDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
        Task<bool> UpdateCustomerInfoAsync(int customerId, UpdateCustomerDto updateDto);
    }
}
