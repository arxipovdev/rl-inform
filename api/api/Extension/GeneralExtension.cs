using System.Linq;
using Microsoft.AspNetCore.Http;

namespace api.Extension
{
    public static class GeneralExtension
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
    }
}