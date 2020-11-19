using System.Linq;
using System.Threading.Tasks;
using api.Contracts.V1.Responses;
using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace api.Extension
{
    public static class GeneralExtension
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
        
        public static async Task<UserResponse> GetUserAsync(this HttpContext httpContext)
        {
            var userId = httpContext.User.Claims.Single(x => x.Type == "id").Value;
            var userManager = httpContext.RequestServices.GetService<UserManager<User>>();
            if (userManager == null) return null;
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);
                return new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Login = user.Login,
                    Roles = roles.OrderBy(x => x)
                };
            }
            return null;
        }
    }
}