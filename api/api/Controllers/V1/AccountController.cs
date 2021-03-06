using System.Linq;
using System.Threading.Tasks;
using api.Contracts.V1;
using api.Contracts.V1.Requests;
using api.Contracts.V1.Responses;
using api.Extension;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.V1
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost(ApiRoutes.Account.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values
                    .SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                
                return BadRequest(new AuthFailedResponse{Errors = errors});
            }
            
            var authResponse = await _accountService.RegisterAsync(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse {Errors = authResponse.Errors});
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        
        [HttpPost(ApiRoutes.Account.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _accountService.LoginAsync(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse {Errors = authResponse.Errors});
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        
        [HttpPost(ApiRoutes.Account.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Values
                    .SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new AuthFailedResponse{Errors = errors});
            }
            var authResponse = await _accountService.ChangePassword(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse {Errors = authResponse.Errors});
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
        
        [HttpPost(ApiRoutes.Account.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await _accountService.RefreshTokenAsync(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse {Errors = authResponse.Errors});
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Account.User)]
        public async Task<IActionResult> CurrentUser()
        {
            var user = await HttpContext.GetUserAsync();
            return Ok(new {user});
        }
    }
}