using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MomNKidStore_BE.Business.ModelViews.CustomerDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IAuthorizeService _authorizeService;

        public CustomerController(ICustomerService customerService, IAuthorizeService authorizeService)
        {
            _customerService = customerService;
            _authorizeService = authorizeService;
        }

        [Authorize(Policy = "RequireAllRoles")]
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomerById(int customerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer && !checkMatchedId.isAuthorizedAccount)
                {
                    return Forbid();
                }
                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer == null)
                {
                    return NotFound("Customer not found");
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomerInfo(int customerId, [FromBody] UpdateCustomerDto updateDto)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(customerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }
                var result = await _customerService.UpdateCustomerInfoAsync(customerId, updateDto);
                if (!result)
                {
                    return StatusCode(500, "Error updating customer info");
                }
                return Ok("Customer info updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
