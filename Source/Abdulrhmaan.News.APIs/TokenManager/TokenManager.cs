using Abdulrhmaan.News.SQlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace Abdulrhmaan.News.APIs.TokenManager
{
    public class TokenManager : ITokenManager
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;




        public TokenManager(UserManager<User> userManager, IDistributedCache cache, IHttpContextAccessor httpContextAccessor, NewsContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentAsync());

        public async Task DeactivateCurrentAsync()
            => await DeactivateAsync(GetCurrentAsync());


        public async Task SaveAccessToken(string token)
        {
            await _cache.SetStringAsync(GetKey(token), " ", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                       TimeSpan.FromMinutes(60)
            });
        }
        // Check that our token is into the whitelist (cached valid tokens) 
        public async Task<bool> IsActiveAsync(string token)
        {
            return await _cache.GetStringAsync(GetKey(token)) != null;
        }
        // here we set a key of tokens we desire to deactivate 
        public async Task DeactivateAsync(string token)
        {
            var TokenToDeactivate = await _cache.GetStringAsync(GetKey(token));
            if (TokenToDeactivate != null)
            {
                await _cache.RemoveAsync(TokenToDeactivate);
            }
        }


        //Get our current Token from the request header 
        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        // return a cach key 
        private static string GetKey(string token)
            => $"tokens:{token}:deactivated";


        public async Task<List<Claim>> GetClaims(User User)
        {
            var claims = new List<Claim>();
            if (User != null)
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,  User.Id ),
                    new Claim(ClaimTypes.Name,  User.UserName),
                    new Claim(ClaimTypes.Email,  User.Email)
                };

                var roles = await _userManager.GetRolesAsync(User);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            return claims;
        }
    }

}
