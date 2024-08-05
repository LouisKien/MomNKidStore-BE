using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MomNKidStore_BE.Business.ModelViews.ProductDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;
using MomNKidStore_Repository.Entities;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _categoryService;
        private readonly string _imagesDirectory;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IProductCategoryService categoryService, IWebHostEnvironment env, IMapper mapper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _imagesDirectory = Path.Combine(env.ContentRootPath, "img", "product");
            _mapper = mapper;
        }

        //[Authorize(Policy = "RequireStaffRole")]
        [HttpPost("add-product")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDtoRequest productView)
        {
            try
            {
                var imagePaths = new List<string>();
                if (productView.Images.Any())
                {
                    foreach (var image in productView.Images)
                    {
                        if (!String.IsNullOrEmpty(image.ImageProduct1))
                        {
                            byte[] imageBytes = Convert.FromBase64String(image.ImageProduct1);
                            string filename = $"ProductImage_{Guid.NewGuid()}.png";
                            string imagePath = Path.Combine(_imagesDirectory, filename);
                            System.IO.File.WriteAllBytes(imagePath, imageBytes);
                            imagePaths.Add(filename);
                        }
                    }
                }
                var product = _mapper.Map<Product>(productView);
                var checkSuccess = await _productService.AddNewProduct(product, imagePaths);
                if (checkSuccess)
                {
                    return Ok("Create successful");
                }
                else
                {
                    return BadRequest("Create fail");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetAllProduct([FromQuery] int CategoryId)
        {
            var products = await _productService.GetAllProducts(CategoryId);
            if (products != null)
            {
                foreach (var product in products)
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
                return Ok(products);
            }
            else
            {
                return NotFound("Products not avaiable");
            }
        }

        [AllowAnonymous]
        [HttpGet("get-product-by-id/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByID(id);
            if (product != null)
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
                return Ok(product);
            }
            else
            {
                return BadRequest("Product not found");
            }
        }

        [AllowAnonymous]
        [HttpGet("search-product/{searchInput}")]
        public async Task<IActionResult> SearchProduct(string searchInput)
        {
            try
            {
                var products = await _productService.Search(searchInput);
                if (products != null)
                {
                    foreach (var product in products)
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
                    return Ok(products);
                }
                else
                {
                    return NotFound("No result");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        //[Authorize(Policy = "RequireStaffRole")]
        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDtoRequest productView, int id)
        {
            var imagePaths = new List<string>();
            if (productView.Images.Any())
            {
                foreach (var image in productView.Images)
                {
                    if (!String.IsNullOrEmpty(image.ImageProduct1))
                    {
                        byte[] imageBytes = Convert.FromBase64String(image.ImageProduct1);
                        string filename = $"ProductImage_{Guid.NewGuid()}.png";
                        string imagePath = Path.Combine(_imagesDirectory, filename);
                        System.IO.File.WriteAllBytes(imagePath, imageBytes);
                        imagePaths.Add(filename);
                    }
                }
            }
            var checkSuccess = await _productService.UpdateProduct(productView, imagePaths, id);
            if (checkSuccess.check && checkSuccess.oldImagePaths != null)
            {
                if (checkSuccess.oldImagePaths.Any())
                {
                    foreach (var oldImagePath in checkSuccess.oldImagePaths)
                    {
                        var fullImagePath = Path.Combine(_imagesDirectory, oldImagePath);
                        if (System.IO.File.Exists(fullImagePath))
                        {
                            System.IO.File.Delete(fullImagePath);
                        }
                    }
                }
            }
            if (checkSuccess.check)
            {
                return Ok("Update successful");
            }
            else
            {
                return BadRequest("Update fail");
            }
        }

        //[Authorize(Policy = "RequireStaffRole")]
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] int id, [FromQuery] int status)
        {
            try
            {
                var check = await _productService.UpdateProductStatus(id, status);
                if (check)
                {
                    return Ok("Update successfully");
                }
                else
                {
                    return BadRequest("Update fail");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var checkSuccess = await _productService.HideProduct(id);
                if (!checkSuccess)
                {
                    return BadRequest("Product does not exist");
                }
                return Ok("Delete successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
