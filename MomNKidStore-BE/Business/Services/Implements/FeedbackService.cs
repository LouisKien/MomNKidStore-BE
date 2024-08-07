using AutoMapper;
using Azure.Core;
using MomNKidStore_BE.Business.ModelViews.FeedbackDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }

        public async Task<bool> CreateFeedback(FeedbackDtoRequest request)
        {
            try
            {
                var shopExist = await _unitOfWork.ProductRepository.FindAsync(x => x.ProductId == request.ProductId && x.ProductStatus == 1);
                
                if (shopExist == null)
                {
                    return false;
                }
                var orderExist = (await _unitOfWork.OrderRepository.FindAsync(x => x.CustomerId == request.CustomerId));
                if (orderExist.Any())
                {
                    foreach (var order in orderExist)
                    {
                        if (order.Status == 1)
                        {
                            var orderDetail = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.ProductId == request.ProductId);
                            if (orderDetail.Any())
                            {
                                var map = _mapper.Map<Feedback>(request);
                                map.Status = true;
                                await _unitOfWork.FeedbackRepository.AddAsync(map);
                                await Task.Delay(500);
                                await _unitOfWork.SaveAsync();


                                return true;
                            }
                            
                        }
                        
                    }
                    
                }
                return false;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<bool> DeleteFeedback(int feedbackId, int customerId)
        {
            try
            {
                var fbExist = (await _unitOfWork.FeedbackRepository.GetAsync(f => f.FeedbackId == feedbackId && f.CustomerId == customerId)).FirstOrDefault();

                if (fbExist == null)
                {
                    return false;
                }

                await _unitOfWork.FeedbackRepository.DeleteAsync(fbExist.FeedbackId);
                await Task.Delay(500);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<IList<FeedbackDtoResponse>> GetAllFeedbackOfProduct(int productId)
        {
            var getAllFbOShop = (await _unitOfWork.FeedbackRepository
                                            .GetAsync(filter: x => x.ProductId == productId
                                                                && x.Status == true,
                                                                includeProperties: "Product,Customer")).ToList();

            var response = _mapper.Map<IList<FeedbackDtoResponse>>(getAllFbOShop);

            return response;
        }

        //public async Task<FeedbackDtoResponse> GetRatingProduct(int productId)
        //{
        //    var verifyAccountId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst("AccountId")?.Value);
        //    var accountExist = await _unitOfWork.CustomerRepository.SingleOrDefaultAsync(x => x.AccountId == verifyAccountId);
        //    var totalRateOfShop = await _unitOfWork.FeedbackRepository
        //                                        .GetAsync(filter: x => x.ProductId == productId
        //                                                            && x.Status == true);
        //    var getRateUrSelf = await _unitOfWork.FeedbackRepository
        //                                        .GetAsync(filter: x => x.ProductId == productId
        //                                        && x.CustomerId == accountExist.CustomerId
        //                                                            && x.Status == true,
        //                                                  includeProperties: "Product");

        //    var getFirst = getRateUrSelf.FirstOrDefault();

        //    var averageNum = totalRateOfShop.Any() ? totalRateOfShop.Average(x => x.RateNumber) : (double?)null;

        //    var response = _mapper.Map<FeedbackDtoResponse>(getFirst);
        //    if (getFirst != null)
        //    {
        //        response.RateNumber = getFirst.RateNumber;
        //    }
        //    else
        //    {
        //        response.RateNumber = 0;
        //    }
        //    response.AverageNumber = averageNum;

        //    return response;
        //}


        public async Task<FeedbackDtoResponse> GetOneFb(int feedbackId)
        {
            var getAll = await _unitOfWork.FeedbackRepository
                                            .GetAsync(filter: x => x.FeedbackId == feedbackId,
                                                                includeProperties: "Product,Customer");
            var final = getAll.FirstOrDefault();
            var response = _mapper.Map<FeedbackDtoResponse>(final);
            return response;
        }

        public async Task<bool> UpdateFeedback(int feedbackId, FeedbackDtoRequest request)
        {


            //var getAll = await _unitOfWork.FeedbackRepository
            //                                .GetAsync(filter: x => x.ProductId == request.ProductId
            //                                                    && x.CustomerId == request.CustomerId
            //                                                    && x.FeedbackId == feedbackId,
            //                                                    includeProperties: "Product,Customer");
            //var final = getAll.FirstOrDefault();

            //if (final == null)
            //{
            //    return null;
            //}

            //var map = _mapper.Map(request, final);

            //await _unitOfWork.FeedbackRepository.UpdateAsync(map);
            //await Task.Delay(500);
            //await _unitOfWork.SaveAsync();

            //var response = _mapper.Map<FeedbackDtoResponse>(map);

            //return response;

            try
            {
                var getAll = await _unitOfWork.FeedbackRepository
                                                .GetAsync(filter: x => x.ProductId == request.ProductId
                                                                    && x.CustomerId == request.CustomerId
                                                                    && x.FeedbackId == feedbackId,
                                                                    includeProperties: "Product,Customer");
                var final = getAll.FirstOrDefault();

                if (final == null)
                {
                    return false;
                }

                var map = _mapper.Map(request, final);

                await _unitOfWork.FeedbackRepository.UpdateAsync(map);
                await Task.Delay(500);
                await _unitOfWork.SaveAsync();


                return true;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<bool> UpdateStsAdmin(int feedbackId)
        {
            try
            {
                var getFeed = await _unitOfWork.FeedbackRepository
                                            .GetByIDAsync(feedbackId);

                if (getFeed != null)
                {
                    if(getFeed.Status == true)
                    {
                        getFeed.Status = false;
                        await _unitOfWork.FeedbackRepository.UpdateAsync(getFeed);
                        await Task.Delay(500);
                        await _unitOfWork.SaveAsync();

                        return true;
                    }
                    else
                    {
                        getFeed.Status = true;
                        await _unitOfWork.FeedbackRepository.UpdateAsync(getFeed);
                        await Task.Delay(500);
                        await _unitOfWork.SaveAsync();

                        return true;
                    }
                    
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            

            
        }
    }
}
