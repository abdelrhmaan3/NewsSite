using Abdulrhmaan.News.SQlServer;
using System.Security.Claims;

namespace Abdulrhmaan.News.APIs.TokenManager;
public interface ITokenManager
{
    Task<bool> IsCurrentActiveToken();
    Task DeactivateCurrentAsync();
    Task<bool> IsActiveAsync(string token);
    Task DeactivateAsync(string token);
    Task<List<Claim>> GetClaims(User User);
    Task SaveAccessToken(string token);
}


