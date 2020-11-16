using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class UserRegistrationRequest
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string Login { get; set; }
        
        [EmailAddress(ErrorMessage = "Email не валидный")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Обязательное поле")]
        public string Password { get; set; }
        
        public string Name { get; set; }
    }
}