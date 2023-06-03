using Abdulrhmaan.News.SQlServer;
using Abdulrhmaan.NewsSite.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Abdulrhmaan.News.APIs.UserServices
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        private User? _user;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUser(RegisterUser userForRegistration)
        {
            var User = userForRegistration.Adapt<User>();
            var LastId = int.Parse(await _userManager.Users.MaxAsync(x => x.Id) ?? "0");
            User.Id = (LastId + 1).ToString();
            var result = await _userManager.CreateAsync(User, userForRegistration.Password);
            return result;
        }


    }

}
