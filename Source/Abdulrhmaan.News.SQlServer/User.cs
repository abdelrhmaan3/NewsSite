using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abdulrhmaan.News.SQlServer;

public class User : IdentityUser
{
    public  string Id { get; set; }
    public string Name { get; set; }
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public  ICollection<News> News { get; } = new List<News>();


}

