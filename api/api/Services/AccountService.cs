using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Contracts.V1.Requests;
using api.Data;
using api.Models;
using api.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class AccountService : IAccountService
    {
        private const string DefaultRole = "Customer";
        private const int ExpiryRefreshToken = 1;
        private readonly UserManager<User> _userManager;
        private readonly JwtOptions _jwtOptions;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;
        private readonly IPasswordValidator<User> _passwordValidator;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(UserManager<User> userManager, JwtOptions jwtOptions,
            TokenValidationParameters tokenValidationParameters, DataContext context,
            IPasswordValidator<User> passwordValidator, IPasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResult> RegisterAsync(UserRegistrationRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                return new AuthResult {Errors = new[] {"Пользователь с таким email уже существует"}};
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
                return new AuthResult {Errors = createdUser.Errors.Select(x => x.Description)};
            }

            // Set Default Role = "Customer"
            await _userManager.AddToRoleAsync(newUser, DefaultRole);

            return await GenerateAuthResultForUserAsync(newUser);
        }

        public async Task<AuthResult> LoginAsync(UserLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return new AuthResult {Errors = new[] {"Email или пароль не корректный!"}};
            }

            var hasValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!hasValidPassword)
            {
                return new AuthResult {Errors = new[] {"Email или пароль не корректный!"}};
            }

            return await GenerateAuthResultForUserAsync(user);
        }

        public async Task<AuthResult> ChangePassword(UpdatePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
            {
                return new AuthResult {Errors = new[] {"Пользователь не найден"}};
            }

            var hasValidPassword = await _userManager.CheckPasswordAsync(user, request.PasswordOld);

            if (!hasValidPassword)
            {
                return new AuthResult {Errors = new[] {"Старый пароль неверный!"}};
            }

            var result = await _passwordValidator.ValidateAsync(_userManager, user, request.Password);

            if (!result.Succeeded)
            {
                return new AuthResult {Errors = result.Errors.Select(x => x.Description)};
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            await _userManager.UpdateAsync(user);
            return await GenerateAuthResultForUserAsync(user);
        }

        public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var validatedToken = GetPrincipalFromToken(request.Token);

            if (validatedToken is null)
            {
                return new AuthResult {Errors = new[] {"Invalid Token"}};
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expireDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expireDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthResult {Errors = new[] {"The token hasn't expired yet"}};
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken =
                await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == request.RefreshToken);

            if (storedRefreshToken is null)
            {
                return new AuthResult {Errors = new[] {"This refresh token doesn't exist"}};
            }

            if (DateTime.UtcNow > storedRefreshToken.Expiry)
            {
                return new AuthResult {Errors = new[] {"This refresh token has expired"}};
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthResult {Errors = new[] {"This refresh token has been invalidated"}};
            }

            if (storedRefreshToken.Used)
            {
                return new AuthResult {Errors = new[] {"This refresh token has been used"}};
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthResult {Errors = new[] {"This refresh token doesn't match this JWT"}};
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var userId = validatedToken.Claims.Single(x => x.Type == "id").Value;
            var user = await _userManager.FindByIdAsync(userId);

            return await GenerateAuthResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

                return !IsJwtValidateAlgorithm(validatedToken) ? null : principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtValidateAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthResult> GenerateAuthResultForUserAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("roles", string.Join(",", roles.OrderBy(x => x))),
                new Claim("id", user.Id)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtOptions.TokenLifeTime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                JwtId = token.Id,
                UserId = user.Id,
                CreateAt = DateTime.UtcNow,
                Expiry = DateTime.UtcNow.AddMonths(ExpiryRefreshToken) // Expiry refreshToken 1 month
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}