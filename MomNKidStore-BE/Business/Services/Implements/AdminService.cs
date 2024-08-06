using AutoMapper;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_BE.Business.ModelViews.DashboardDTOs;
using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateAccountStaff(UserRegisterDtoRequest newAccount)
        {
            try
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        newAccount.Password = await HashPassword(newAccount.Password);
                        var account = _mapper.Map<Account>(newAccount);
                        account.Status = true;
                        account.RoleId = 2;
                        await _unitOfWork.AccountRepository.AddAsync(account);
                        await _unitOfWork.SaveAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                var insertedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.Email == newAccount.Email)).FirstOrDefault();
                if (insertedAccount != null)
                {
                    await _unitOfWork.AccountRepository.DeleteAsync(insertedAccount);
                    await _unitOfWork.SaveAsync();
                }
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> HashPassword(string password)
        {
            try
            {
                using (SHA512 sha512 = SHA512.Create())
                {
                    byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        stringBuilder.Append(hashBytes[i].ToString("x2"));
                    }

                    return await Task.FromResult(stringBuilder.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> LockAccount(int accountId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                    if (account == null)
                    {
                        throw new ArgumentException($"Account not found for account {accountId}");
                    }
                    account.Status = false;

                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (ArgumentException)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<bool> UnlockAccount(int accountId)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var account = await _unitOfWork.AccountRepository.GetByIDAsync(accountId);
                    if (account == null)
                    {
                        throw new ArgumentException($"Account not found for account {accountId}");
                    }
                    account.Status = true;

                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                    await _unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    return true;
                }
                catch (ArgumentException)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<DashboardDTO> GetDashboard()
        {
            try
            {
                var response = new DashboardDTO();
                var productSales = new Dictionary<int, int>();

                var orders = await _unitOfWork.OrderRepository.GetAllAsync();
                if (orders.Any()) {
                    foreach (var order in orders)
                    {
                        if (order.Status == 1 || order.Status == 3 || order.Status == 4)
                        {
                            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == order.OrderId);
                            if (orderDetails.Any())
                            {
                                foreach (var orderDetail in orderDetails)
                                {
                                    if (!productSales.ContainsKey(orderDetail.ProductId))
                                    {
                                        productSales[orderDetail.ProductId] = 0;
                                    }
                                    productSales[orderDetail.ProductId] += orderDetail.OrderQuantity;
                                    response.totalSoldProduct += orderDetail.OrderQuantity;
                                }
                            }
                        }
                        response.totalRevenue += order.TotalPrice;
                        response.totalOrder++;
                    }
                }

                var topSellingProducts = productSales.OrderByDescending(ps => ps.Value).Take(5).ToDictionary(ps => ps.Key, ps => ps.Value);

                foreach (var product in topSellingProducts)
                {
                    var pd = await _unitOfWork.ProductRepository.GetByIDAsync(product.Key);
                    var productResponse = _mapper.Map<ProductDtoResponse>(pd);
                    var productImages = (await _unitOfWork.ImageProductRepository.GetAsync(p => p.ProductId == pd.ProductId)).FirstOrDefault();
                    if (productImages != null)
                    {
                        var image = new ImageProductDto
                        {
                            ImageProduct1 = productImages.ImageProduct1
                        };
                        productResponse.Images.Add(image);
                    }
                    response.topSellingProducts.Add(productResponse);
                }

                return response;
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
