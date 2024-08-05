using MomNKidStore_BE.Business.ModelViews.PaymentDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDtoResponse> CreatePayment(PaymentDtoRequest paymentRequest);
        Task<PaymentDtoResponse> CancelTransaction(PaymentDtoRequest paymentRequest);
    }
}
