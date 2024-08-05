using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;

        public AdminController(IAdminService adminService, IAuthService authService)
        {
            _adminService = adminService;
            _authService = authService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("/api/v1/accounts/create-staff")]
        public async Task<IActionResult> CreateAccountStaff([FromBody] UserRegisterDtoRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.UserName))
                {
                    return BadRequest("Please fill at least Username, email and password fields");
                }
                if (!request.Password.Equals(request.ConfirmPassword))
                {
                    return BadRequest("Not matching password");
                }
                if (!await _authService.GetAccountByEmail(request.Email))
                {
                    bool checkCreated = await _adminService.CreateAccountStaff(request);
                    if (checkCreated)
                    {
                        return Ok("Create success");
                    }
                    else
                    {
                        return BadRequest("Not correct role");
                    }
                }
                else
                {
                    return BadRequest("Existed email");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("/api/v1/lock-account")]
        public async Task<IActionResult> LockAccount(int accountId)
        {
            try
            {
                bool check = await _adminService.LockAccount(accountId);
                if (check)
                {
                    return Ok("Lock account success");
                }
                else
                {
                    return BadRequest("Account not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("/api/v1/unlock-account")]
        public async Task<IActionResult> UnlockAccount(int accountId)
        {
            try
            {
                bool check = await _adminService.UnlockAccount(accountId);
                if (check)
                {
                    return Ok("Unlock account success");
                }
                else
                {
                    return BadRequest("Account not found");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
