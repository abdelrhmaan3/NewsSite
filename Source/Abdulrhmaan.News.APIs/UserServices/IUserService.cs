using Abdulrhmaan.NewsSite.Data;
using Microsoft.AspNetCore.Identity;

namespace Abdulrhmaan.News.APIs.UserServices
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterUser(RegisterUser userForRegistration);

    }
}
