using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Login Обязательное поле")]
        public string Login { get; set; }
        
        [EmailAddress(ErrorMessage = "Email адрес не валидный")]
        public string Email { get; set; }
        
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Password Обязательное поле")]
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
        
        [RoleValid]
        public IEnumerable<string> Roles { get; set; }
    }
}