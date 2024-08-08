using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MomNKidStore_BE.Business.ModelViews.FeedbackDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("GetAllFeedback/{productId}")]
        public async Task<IActionResult> GetAllFeedbackOfProduct(int productId)
        {
            try
            {
                var getAll = await _service.GetAllFeedbackOfProduct(productId);
                if (getAll.IsNullOrEmpty())
                {
                    return Ok("Feedback is empty !!");
                }

                return Ok(getAll);
            }
            catch
            {
                return BadRequest("Valid");
            }

        }

        //[AllowAnonymous]
        //[HttpGet("GetRate/{productId}")]
        //public async Task<IActionResult> GetRatingShop(int productId)
        //{
        //    try
        //    {
        //        var getOneRating = await _service.GetRatingProduct(productId);
        //        if (getOneRating == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(getOneRating);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [AllowAnonymous]
        [HttpGet("GetOneFeedback/{feedbackId}")]
        public async Task<IActionResult> GetOneFb(int feedbackId)
        {
            try
            {
                var getOne = await _service.GetOneFb(feedbackId);
                if (getOne == null)
                {
                    return Ok("Feedback not found !!");
                }

                return Ok(getOne);
            }
            catch
            {
                return BadRequest("Valid");
            }
        }

        //[Authorize(Policy = "RequireCustomerRole")]
        [AllowAnonymous]
        [HttpPost("CreateFeedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDtoRequest request)
        {
            try
            {
                //var customerId = User.FindFirst("CustomerId")?.Value;
                //if (customerId == null)
                //{
                //    return Forbid();
                //}
                //var checkMatchedId = await _authorizeService.CheckAuthorizeByCartId(request.CustomerId, int.Parse(customerId));
                //if (!checkMatchedId)
                //{
                //    return Forbid();
                //}
                var response = await _service.CreateFeedback(request);
                if(response)
                {
                    return Ok("Successuflly");
                }
                else
                {
                    return BadRequest("Unsuccessfully");
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("UpdateFeedback/{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback(int feedbackId, [FromBody] FeedbackDtoRequest request)
        {
            try
            {
                //var customerId = User.FindFirst("CustomerId")?.Value;
                //if (customerId == null)
                //{
                //    return Forbid();
                //}
                //var checkMatchedId = await _authorizeService.CheckAuthorizeByCartId(feedbackId, int.Parse(customerId));
                //if (!checkMatchedId)
                //{
                //    return Forbid();
                //}
                var response = await _service.UpdateFeedback(feedbackId, request);
                if (response)
                {
                    return Ok("Successfully");
                }
                else
                {
                    return BadRequest("Unsuccessfully");
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("UpdateStsAdmin/{feedbackId}")]
        public async Task<IActionResult> UpdateStsAdmin(int feedbackId)
        {
            try
            {
                var response = await _service.UpdateStsAdmin(feedbackId);
                if (response)
                {
                    return Ok("Change status successfully");
                }
                return BadRequest("Invalid");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpDelete("{feedbackId}/{customerId}")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId, int customerId)
        {
            try
            {
                //var customerId = User.FindFirst("CustomerId")?.Value;
                //if (customerId == null)
                //{
                //    return Forbid();
                //}
                //var checkMatchedId = await _authorizeService.CheckAuthorizeByCartId(feedbackId, int.Parse(customerId));
                //if (!checkMatchedId)
                //{
                //    return Forbid();
                //}
                var response = await _service.DeleteFeedback(feedbackId, customerId);
                if (response)
                {
                    return Ok("Delete successfully");
                }
                return BadRequest("Invalid");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
