using System.Threading.Tasks;
using api.Contracts.V1.Requests;
using api.Models;

namespace api.Services
{
    public interface IAccountService
    {
        Task<AuthResult> RegisterAsync(UserRegistrationRequest request);
        Task<AuthResult> LoginAsync(UserLoginRequest request);
    }
}