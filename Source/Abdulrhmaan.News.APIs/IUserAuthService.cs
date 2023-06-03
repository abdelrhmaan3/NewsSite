using Abdulrhmaan.NewsSite.Data;
using Microsoft.AspNetCore.Identity;

namespace Abdulrhmaan.News.APIs
{
    public interface IUserAuthService
    {
        Task<IdentityResult> RegisterUser(RegisterUser userForRegistration);
        Task<bool> ValidateUser(LoginUser userForAuth);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);

    }
}
