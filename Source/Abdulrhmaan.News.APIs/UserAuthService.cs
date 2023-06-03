using Abdulrhmaan.News.SQlServer;
using Abdulrhmaan.NewsSite.Data;
using Abdulrhmaan.NewsSite.Data.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Abdulrhmaan.News.APIs
{
    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        private User? _user;

        public UserAuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterUser(RegisterUser userForRegistration)
        {
            var User = userForRegistration.Adapt<User>();

            var result = await _userManager.CreateAsync(User, userForRegistration.Password);
            return result;
        }

        public async Task<bool> ValidateUser(LoginUser userForAuth)
        {
            _user = await _userManager.FindByNameAsync(userForAuth.UserName);
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password));
            return result;
        }
        public async Task<TokenDto> CreateToken(bool populateExp)
        {
            var Claims = await GetClaims();

            var TokenOptions = new JwtSecurityToken
                             (_configuration.GetValue<string>("Jwt:Issuer"),
                             _configuration.GetValue<string>("Jwt:Audience"),
                             Claims,
                             expires: DateTime.UtcNow.AddMinutes(60),
                             signingCredentials: new SigningCredentials(
                             new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key"))),
                             SecurityAlgorithms.HmacSha256)
                              );

            var refreshToken = GenerateRefreshToken();
            if (_user != null)
            {
                _user.RefreshToken = refreshToken;
                if (populateExp)
                    _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _userManager.UpdateAsync(_user);
            }
            var accessToken = new JwtSecurityTokenHandler().WriteToken(TokenOptions);
            return new TokenDto(accessToken, refreshToken);
        }
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>();
            if (_user != null)
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,  _user.Id ),
                    new Claim(ClaimTypes.Name,  _user.UserName)
                };

                var roles = await _userManager.GetRolesAsync(_user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return claims;
        }




        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key") ?? throw new ArgumentNullException($" IssuerSigningKey is null"))),
                ValidateLifetime = true,
                ValidIssuer = _configuration.GetValue<string>("Jwt:Issuer"),
                ValidAudience = _configuration.GetValue<string>("Jwt:Audience")

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var user = await _userManager.FindByNameAsync(principal?.Identity?.Name);
            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequest();
            _user = user;

            return await CreateToken(populateExp: false);
        }
    }

}
