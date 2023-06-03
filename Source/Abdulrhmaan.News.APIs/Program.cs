using Abdulrhmaan.News.APIs;
using Abdulrhmaan.News.APIs.Mapster;
using Abdulrhmaan.News.APIs.TokenManager;
using Abdulrhmaan.News.APIs.UserServices;
using Abdulrhmaan.News.SQlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container(configuring Services).

builder.Services.AddDbContext<NewsContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>

    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        };

        // We have to hook the OnMessageReceived event in order to 
        // allow the JWT authentication handler to read the access
        // token from the query string as Im using Bearer token authentication upon signalR 
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/hubs/chat")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };

    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.RegisterMapsterConfiguration();
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<NewsContext>();
builder.Services.AddTransient<ITokenManager, TokenManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedRedisCache(x =>
{
    x.Configuration = builder.Configuration["redis:connectionString"];
    //x.ConfigurationOptions = new ConfigurationOptions
    //{    
    //    Ssl = true,
    //    AbortOnConnectFail = false,
    //    AllowAdmin = true,
    //    ConnectTimeout = 30000,
    //    SyncTimeout = 30000
    //};
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// minimanl apis
APIRouteHandler.RegisterAuthenticationAPIs(app);


app.Run();

