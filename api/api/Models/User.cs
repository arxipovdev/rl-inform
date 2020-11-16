using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Login { get; set; }
        public string Name { get; set; }
    }
}