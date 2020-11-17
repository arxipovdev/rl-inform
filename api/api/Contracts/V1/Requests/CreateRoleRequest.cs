using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "Name Обязательное поле")]
        public string Name { get; set; }
    }
}