using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Contracts.V1.Requests;
using api.Models;
using api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AccountService(UserManager<User> userManager, JwtOptions jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }
        public async Task<AuthResult> RegisterAsync(UserRegistrationRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new AuthResult{Errors = new []{"Пользователь с таким email уже существует"}};
            }

            var newUser = new User
            {
                Login = request.Login,
                UserName = request.Login,
                Email = request.Email,
                Name = request.Name
            };
            
            var createdUser = await _userManager.CreateAsync(newUser, request.Password);
            
            if (!createdUser.Succeeded)
            {
                return new AuthResult{Errors = createdUser.Errors.Select(x => x.Description)};
            }
            
            return GenerateAuthResultForUser(newUser);
        }

        public async Task<AuthResult> LoginAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return new AuthResult {Errors = new[] {"Email или пароль не корректны!"}};
            }

            var hasValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!hasValidPassword)
            {
                return new AuthResult {Errors = new[] {"Email или пароль не корректны!"}};
            }

            return GenerateAuthResultForUser(user);
        }

        private AuthResult GenerateAuthResultForUser(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)
                }),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}