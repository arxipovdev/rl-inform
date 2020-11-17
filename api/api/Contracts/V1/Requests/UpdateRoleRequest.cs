using System.ComponentModel.DataAnnotations;

namespace api.Contracts.V1.Requests
{
    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "Name Обязательное поле")]
        public string Name { get; set; }
    }
}