using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MomNKidStore_BE.Business.ModelViews.CartDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartItemService;
        private readonly IAuthorizeService _authorizeService;
        private readonly string _imagesDirectory;

        public CartController(ICartService cartItemService, IAuthorizeService authorizeService, IWebHostEnvironment env)
        {
            _cartItemService = cartItemService;
            _authorizeService = authorizeService;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "product");
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartDtoRequest request)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(request.CustomerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }
                if (request == null)
                {
                    return BadRequest("Cannot add empty object to cart");
                }
                var status = await _cartItemService.AddToCart(request);
                if (status)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Cannot add this project to cart because of larger quantity than in stock");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpGet("{CustomerId}")]
        public async Task<IActionResult> GetCustomerCart(int CustomerId)
        {
            try
            {
                var accountId = User.FindFirst("AccountId")?.Value;
                if (accountId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCustomerId(CustomerId, int.Parse(accountId));
                if (!checkMatchedId.isMatchedCustomer)
                {
                    return Forbid();
                }
                var response = await _cartItemService.GetCartByCustomerId(CustomerId);
                if (!response.Any())
                {
                    return Ok(response);
                }
                foreach (var item in response)
                {
                    if (item.ProductView.Images.Any())
                    {
                        foreach (var image in item.ProductView.Images)
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

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemInCart(int id)
        {
            try
            {
                var customerId = User.FindFirst("CustomerId")?.Value;
                if (customerId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCartId(id, int.Parse(customerId));
                if (!checkMatchedId)
                {
                    return Forbid();
                }
                var check = await _cartItemService.DeleteItemInCart(id);
                if (check)
                {
                    return Ok("Delete successfully");
                }
                else
                {
                    return BadRequest("Item does not exist");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "RequireCustomerRole")]
        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateItemQuantityInCart([FromQuery] int CartId, [FromQuery] int Quantity)
        {
            try
            {
                var customerId = User.FindFirst("CustomerId")?.Value;
                if (customerId == null)
                {
                    return Forbid();
                }
                var checkMatchedId = await _authorizeService.CheckAuthorizeByCartId(CartId, int.Parse(customerId));
                if (!checkMatchedId)
                {
                    return Forbid();
                }
                var response = await _cartItemService.UpdateItemQuantityInCart(CartId, Quantity);
                if (response == 1)
                {
                    return Ok("Update quantity success");
                }
                else if (response == 3)
                {
                    return Ok("Remove item success");
                }
                else if (response == 2)
                {
                    return BadRequest("Your quantity is greater than number of product in stock");
                }
                else if (response == -1)
                {
                    return BadRequest("Product is not exist");
                }
                else if (response == 0)
                {
                    return BadRequest("Item in cart is not exist");
                }
                return BadRequest("Cannot update");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
