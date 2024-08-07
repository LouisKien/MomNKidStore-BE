using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;
        private readonly string _imagesDirectory;

        public AdminController(IAdminService adminService, IAuthService authService, IWebHostEnvironment env)
        {
            _adminService = adminService;
            _authService = authService;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "product");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("all-account")]
        public async Task<IActionResult> GetAccountList(int Role)
        {
            try
            {
                var response = await _adminService.GetAccountList(Role);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateAccountStaff([FromBody] StaffDtoRequest request)
        {
            try
            {
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

        [AllowAnonymous]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var response = await _adminService.GetDashboard();
                foreach (var product in response.topSellingProducts)
                {
                    if (product.Images.Any())
                    {
                        foreach (var image in product.Images)
                        {
                            var imagePath = Path.Combine(_imagesDirectory, image.ImageProduct1);
                            if (System.IO.File.Exists(imagePath))
                            {
                                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);
                                image.ImageProduct1 = Convert.ToBase64String(imageBytes);
                            }
                        }
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
