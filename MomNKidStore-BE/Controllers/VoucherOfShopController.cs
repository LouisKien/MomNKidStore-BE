using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MomNKidStore_BE.Business.ModelViews.VoucherOfShopDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VoucherOfShopController : ControllerBase
    {
        private readonly IVoucherOfShopService _voucherOfShopService;
        public VoucherOfShopController(IVoucherOfShopService voucherOfShopService)
        {
            _voucherOfShopService = voucherOfShopService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _voucherOfShopService.Get();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{voucherId}")]
        public async Task<IActionResult> Get(int voucherId)
        {
            try
            {
                var response = await _voucherOfShopService.Get(voucherId);
                if (response == null)
                {
                    return NotFound($"Cannot find voucher match id {voucherId}");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("staff")]
        public async Task<IActionResult> GetBystaff()
        {
            try
            {
                var response = await _voucherOfShopService.GetByAdmin();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpGet("staff/{voucherId}")]
        public async Task<IActionResult> GetByStaff(int voucherId)
        {
            try
            {
                var response = await _voucherOfShopService.GetByAdmin(voucherId);
                if (response == null)
                {
                    return NotFound($"Cannot find voucher match id {voucherId}");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPost]
        public async Task<IActionResult> Post(VoucherOfShopDtoRequest voucherOfShopDTORequest)
        {
            try
            {
                if (voucherOfShopDTORequest == null)
                {
                    return BadRequest("VoucherOfShop is null");
                }
                if (voucherOfShopDTORequest.StartDate > voucherOfShopDTORequest.EndDate)
                {
                    return BadRequest("EndDate cannot be sooner than StartDate");
                }
                await _voucherOfShopService.Post(voucherOfShopDTORequest);
                return Ok("Create success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("{voucherId}")]
        public async Task<IActionResult> Put(int voucherId, [FromBody] VoucherOfShopDtoRequest voucherOfShopDTORequest)
        {
            try
            {
                if (voucherOfShopDTORequest == null)
                {
                    return BadRequest("VoucherOfShop is null");
                }
                if (voucherOfShopDTORequest.StartDate > voucherOfShopDTORequest.EndDate)
                {
                    return BadRequest("EndDate cannot be sooner than StartDate");
                }
                var response = await _voucherOfShopService.Put(voucherId, voucherOfShopDTORequest);
                if (response)
                {
                    return Ok("Update success");
                }
                else
                {
                    return NotFound($"Cannot find voucher match id {voucherId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut()]
        public async Task<IActionResult> UpdateStatus([FromQuery] int voucherId, [FromQuery] bool status)
        {
            try
            {
                var response = await _voucherOfShopService.UpdateStatus(voucherId, status);
                if (response)
                {
                    return Ok("Update success");
                }
                else
                {
                    return NotFound($"Cannot find voucher match id {voucherId}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
