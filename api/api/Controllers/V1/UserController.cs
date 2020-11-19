using System.Linq;
using System.Threading.Tasks;
using api.Contracts.V1;
using api.Contracts.V1.Requests;
using api.Contracts.V1.Responses;
using api.Extension;
using api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers.V1
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Editor")]
        [HttpGet(ApiRoutes.Users.GetAll)]
        public async Task<IActionResult> Get()
        {
            var users = await _userManager.Users.ToListAsync();
            var response = users.Select(x => new UserResponse
            {
                Id = x.Id,
                Name = x.Name,
                Login = x.Login,
                Email = x.Email
            });
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Editor")]
        [HttpGet(ApiRoutes.Users.Get)]
        public async Task<IActionResult> Get(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new ErrorsResponse{Errors = new []{"User not found"}});
            }

            var roles = await _userManager.GetRolesAsync(user);
            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login,
                Email = user.Email,
                Roles = roles
            };
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Editor")]
        [HttpPost(ApiRoutes.Users.Create)]
        public async Task<IActionResult> Post([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ErrorsResponse{Errors = errors});
            }
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return BadRequest(new ErrorsResponse{Errors = new []{"Пользователь с таким email уже существует"}});
            }

            var user = new User
            {
                Login = request.Login,
                UserName = request.Login,
                Email = request.Email,
                Name = request.Name
            };
            
            var createdUser = await _userManager.CreateAsync(user, request.Password);

            if (!createdUser.Succeeded)
            {
                return BadRequest(new ErrorsResponse {Errors = createdUser.Errors.Select(x => x.Description)});
            }
            await _userManager.AddToRolesAsync(user, request.Roles);
            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Editor,Customer")]
        [HttpPut(ApiRoutes.Users.Update)]
        public async Task<IActionResult> Put(string userId, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ErrorsResponse{Errors = errors});
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return NotFound(new ErrorsResponse{Errors = new []{"Пользователь не существует"}});
            }

            user.Name = request.Name;
            user.Email = request.Email;
            await _userManager.UpdateAsync(user);

            var currentUser = await HttpContext.GetUserAsync();
            var isAdmin = currentUser.Roles.Contains("Admin");
            var isEditor = currentUser.Roles.Contains("Editor");
            if (isAdmin || isEditor)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = request.Roles.Except(userRoles);
                var removedRoles = userRoles.Except(request.Roles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };
            return Ok(response);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpDelete(ApiRoutes.Users.Delete)]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new ErrorsResponse{Errors = new []{"Пользователь не существует"}});
            }

            var deleted = await _userManager.DeleteAsync(user);
            if (!deleted.Succeeded)
            {
                return BadRequest(new ErrorsResponse {Errors = deleted.Errors.Select(x => x.Description)});
            }
            return Ok(user);
        }
    }
}