using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class UserRegistrationRequest
    {
        [Required(ErrorMessage = "Логин обязательное поле")]
        public string Login { get; set; }
        
        [EmailAddress(ErrorMessage = "Email не валидный")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Пароль обязательное поле")]
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
        
        public string Name { get; set; }
    }
}