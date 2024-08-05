using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;
using MomNKidStore_Repository.UnitOfWorks.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MomNKidStore_BE.Business.Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserAuthenticatingDtoResponse?> AuthenticateUser(UserAuthenticatingDtoRequest loginInfo)
        {
            try
            {
                UserAuthenticatingDtoResponse response = new UserAuthenticatingDtoResponse();
                string hashedPassword = await HashPassword(loginInfo.Password);
                var account = (await _unitOfWork.AccountRepository.FindAsync(a => a.Email == loginInfo.Email && a.Password == hashedPassword)).FirstOrDefault();
                if (account != null)
                {
                    response = _mapper.Map<UserAuthenticatingDtoResponse>(account);
                    return response;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GenerateAccessToken(UserAuthenticatingDtoResponse account)
        {
            try
            {
                int role = 0;
                if (account.RoleId == 3)
                {
                    var customer = (await _unitOfWork.CustomerRepository.GetAsync(c => c.AccountId == account.AccountId)).FirstOrDefault();
                    if (customer != null)
                    {
                        role = customer.CustomerId;
                    }
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var accessClaims = new List<Claim>
                {
                    new Claim("AccountId", account.AccountId.ToString()),
                    new Claim("CustomerId", role.ToString()),
                    new Claim("RoleId", account.RoleId.ToString())
                };
                var accessExpiration = DateTime.Now.AddMinutes(30);
                var accessJwt = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], accessClaims, expires: accessExpiration, signingCredentials: credentials);
                var accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);
                return accessToken;
            }
            catch (Exception ex)
            {
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

        public async Task<bool> GetAccountByEmail(string email)
        {
            try
            {
                var account = (await _unitOfWork.AccountRepository.GetAsync(c => c.Email == email)).FirstOrDefault();
                if (account == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> CreateAccountCustomer(UserRegisterDtoRequest newAccount)
        {
            try
            {
                bool status = false;
                newAccount.Password = await HashPassword(newAccount.Password);
                var account = _mapper.Map<Account>(newAccount);
                account.Status = true;
                account.RoleId = 3;
                await _unitOfWork.AccountRepository.AddAsync(account);
                await _unitOfWork.SaveAsync();
                var insertedAccount = (await _unitOfWork.AccountRepository.FindAsync(a => a.Email == newAccount.Email)).FirstOrDefault();
                if (insertedAccount != null)
                {

                    var customer = new Customer
                    {
                        AccountId = insertedAccount.AccountId,
                        UserName = newAccount.UserName,
                        Phone = newAccount.Phone,
                        Address = newAccount.Address,
                        Dob = newAccount.Dob,
                        Point = 0,
                        Status = true
                    };
                    await _unitOfWork.CustomerRepository.AddAsync(customer);
                    await _unitOfWork.SaveAsync();
                    status = true;
                    return status;
                }
                return status;
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
    }
}
