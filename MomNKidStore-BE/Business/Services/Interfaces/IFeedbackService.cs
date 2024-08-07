using MomNKidStore_BE.Business.ModelViews.FeedbackDTOs;

namespace MomNKidStore_BE.Business.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IList<FeedbackDtoResponse>> GetAllFeedbackOfProduct(int productId);
        Task<bool> CreateFeedback(FeedbackDtoRequest request);
        //Task<FeedbackDtoResponse> GetRatingProduct(int productId);
        Task<FeedbackDtoResponse> GetOneFb(int feefbackId);
        Task<bool> UpdateFeedback(int feedbackId, FeedbackDtoRequest request);
        Task<bool> DeleteFeedback(int feedbackId, int customerId);
        Task<bool> UpdateStsAdmin(int feedbackId);
    }
}
