#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace api.Contracts.V1
{
    public class RoleValid : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var roleManager = context.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = roleManager.Roles.Select(x => x.Name).ToList();
            if (!(value is List<string> requestRoles)) return new ValidationResult("Не валидные роли");
            return requestRoles.Any(role => !roles.Contains(role))
                ? new ValidationResult("Не валидные роли")
                : ValidationResult.Success;
        }
    }
}