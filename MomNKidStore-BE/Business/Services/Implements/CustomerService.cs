using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.CustomerDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                if (customer == null) return null;

                return _mapper.Map<CustomerDto>(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCustomerByIdAsync");
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateCustomerInfoAsync(int customerId, UpdateCustomerDto updateDto)
        {
            try
            {
                var existingCustomer = await _unitOfWork.CustomerRepository.GetByIDAsync(customerId);
                if (existingCustomer == null) return false;

                _mapper.Map(updateDto, existingCustomer);
                await _unitOfWork.CustomerRepository.UpdateAsync(existingCustomer);
                await _unitOfWork.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCustomerInfoAsync");
                throw new Exception(ex.Message);
            }
        }
    }
}
