using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class UpdatePasswordRequest
    {
        [Required(ErrorMessage = "UserId Обязательное поле")]
        public string UserId { get; set; }
        
        [Required(ErrorMessage = "PasswordOld Обязательное поле")]
        public string PasswordOld { get; set; }
        
        [Required(ErrorMessage = "Password Обязательное поле")]
        public string Password { get; set; }
        
        [Compare("Password", ErrorMessage = "Новые пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}