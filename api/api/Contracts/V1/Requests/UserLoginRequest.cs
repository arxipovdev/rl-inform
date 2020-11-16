using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        public string Password { get; set; }
    }
}