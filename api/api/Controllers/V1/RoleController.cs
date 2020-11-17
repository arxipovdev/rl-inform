using System.Linq;
using System.Threading.Tasks;
using api.Contracts.V1;
using api.Contracts.V1.Requests;
using api.Contracts.V1.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Editor")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet(ApiRoutes.Roles.GetAll)]
        public async Task<ActionResult> Get()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var response = roles.Select(x => new {x.Id, x.Name});
            return Ok(response);
        }

        [HttpGet(ApiRoutes.Roles.Get)]
        public async Task<ActionResult> Get(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                return NotFound(new ErrorsResponse {Errors = new[] {"Роль не найедна"}});
            }

            var response = new {role.Id, role.Name};
            return Ok(response);
        }

        [HttpPost(ApiRoutes.Roles.Create)]
        public async Task<ActionResult> Post([FromBody] CreateRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ErrorsResponse{Errors = errors});
            }

            var role = await _roleManager.FindByNameAsync(request.Name);
            if (role != null)
            {
                return BadRequest(new ErrorsResponse{Errors = new []{$"Роль '{request.Name}' уже существует"}});
            }

            var created = await _roleManager.CreateAsync(new IdentityRole(request.Name));

            if (!created.Succeeded)
            {
                return BadRequest(new ErrorsResponse{Errors = created.Errors.Select(x => x.Description)});
            }

            role = await _roleManager.FindByNameAsync(request.Name);
            var response = new {role.Id, role.Name};
            return Ok(response);
        }

        [HttpPut(ApiRoutes.Roles.Update)]
        public async Task<ActionResult> Put(string roleId, [FromBody] UpdateRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new ErrorsResponse{Errors = errors});
            }
            
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                return NotFound(new ErrorsResponse{Errors = new []{$"Роль не существует"}});
            }

            role.Name = request.Name;
            await _roleManager.UpdateAsync(role);
            var response = new {role.Id, role.Name};
            
            return Ok(response);
        }

        [HttpDelete(ApiRoutes.Roles.Delete)]
        public async Task<ActionResult> Delete(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                return NotFound(new ErrorsResponse{Errors = new []{$"Роль не существует"}});
            }

            await _roleManager.DeleteAsync(role);
            
            var response = new {role.Id, role.Name};
            return Ok(response);
        }
    }
}