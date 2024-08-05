using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MomNKidStore_BE.Business.ModelViews.AccountDTOs;
using MomNKidStore_BE.Business.Services.Interfaces;

namespace MomNKidStore_BE.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> Login([FromBody] UserAuthenticatingDtoRequest loginInfo)
        {
            try
            {
                IActionResult response = Unauthorized();
                var isAuthenticated = await _authService.AuthenticateUser(loginInfo);
                if (isAuthenticated != null)
                {
                    var accessToken = await _authService.GenerateAccessToken(isAuthenticated);
                    if (accessToken.IsNullOrEmpty())
                    {
                        return BadRequest("Something went wrong");
                    }
                    response = Ok(new { accessToken = accessToken });
                    return response;
                }
                return NotFound("Wrong email or password");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpPost("register-customer")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDtoRequest newAccount)
        {
            try
            {
                if (!newAccount.Password.Equals(newAccount.ConfirmPassword))
                {
                    return BadRequest("Not matching password");
                }
                if (!await _authService.GetAccountByEmail(newAccount.Email))
                {
                    bool checkRegister = await _authService.CreateAccountCustomer(newAccount);
                    if (checkRegister)
                    {
                        return Ok("Create success");
                    }
                    else
                    {
                        return BadRequest("Cannot create account");
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
    }
}
