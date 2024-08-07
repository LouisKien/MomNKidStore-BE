using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MomNKidStore_BE.Business.ModelViews.BlogDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _service;

        public BlogController(IBlogService service)
        {
            _service = service;
        }

        [HttpGet("GetAllBlogByBlogId/{blogId}")]
        public async Task<IActionResult> GetAllBlogByBlogId(int blogId)
        {
            try
            {
                var getAll = await _service.GetAllBlogByBlogId(blogId);
                if (getAll == null)
                {
                    return Ok("Blog is empty !!");
                }

                return Ok(getAll);
            }
            catch
            {
                return BadRequest("Valid");
            }

        }

        [AllowAnonymous]
        [HttpGet("GetAllBlog")]
        public async Task<IActionResult> GetAllBlog()
        {
            try
            {
                var getAll = await _service.GetAllBlog();
                if (getAll.IsNullOrEmpty())
                {
                    return Ok("Blog is empty !!");
                }

                return Ok(getAll);
            }
            catch
            {
                return BadRequest("Valid");
            }

        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPost("createBlog")]
        public async Task<IActionResult> CreateBlog([FromBody] BlogProductDto blogItems)
        {
            try
            {
                var checkItem = await _service.ValidateProductOfBlog(blogItems);
                if (checkItem == -1)
                {
                    return BadRequest("Some items that not valid in your Product");
                }
                else
                {
                    var url = await _service.CreateBlog(blogItems);
                    return Ok(new { url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("updateBlog/{blogId}")]
        public async Task<IActionResult> UpdateBlog(int blogId, [FromBody] BlogProductDto blogItems)
        {
            try
            {
                var checkItem = await _service.ValidateProductOfBlog(blogItems);
                if (checkItem == -1)
                {
                    return BadRequest("Some items that not valid in your Product");
                }
                else
                {
                    var url = await _service.UpdateBlog(blogId, blogItems);
                    return Ok(new { url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireStaffRole")]
        [HttpPut("updateBlogStatus/{blogId}")]
        public async Task<IActionResult> UpdateStatusBlog(int blogId)
        {
            try
            {
                var check = await _service.statusBlog(blogId);
                if (check)
                {
                    return Ok("Update successfully");
                }
                else
                {
                    return BadRequest("Update unsuccessfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
