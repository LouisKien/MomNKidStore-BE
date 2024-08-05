using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MomNKidStore_BE.Business.ModelViews.ProductCategoryDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _categoryService;
        public ProductCategoryController(IProductCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/categories")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                if (categories == null)
                {
                    return NotFound("Categories not found");
                }
                return Ok(categories);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(id);
                if (category == null)
                {
                    return NotFound("No category match this id");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrStaffRole")]
        [HttpPost()]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDtoRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Category cannot null");
                }
                if (request.ProductCategoryName.IsNullOrEmpty())
                {
                    return BadRequest("Please fill all fields");
                }
                await _categoryService.CreateCategory(request);
                return Ok("Create category successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrStaffRole")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryDtoRequest request, int id)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Category cannot null");
                }
                if (request.ProductCategoryName.IsNullOrEmpty())
                {
                    return BadRequest("Please fill all fields");
                }
                await _categoryService.UpdateCategory(id, request);
                return Ok("Update category successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireAdminOrStaffRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.HideCategoryAndProduct(id);
                return Ok("Hide category and all product in category successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
