using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class UpdateUserRequest
    {
        [EmailAddress(ErrorMessage = "Email адрес не валидный")]
        public string Email { get; set; }
        
        public string Name { get; set; }
        
        [RoleValid]
        public IEnumerable<string> Roles { get; set; }
        
    }
}