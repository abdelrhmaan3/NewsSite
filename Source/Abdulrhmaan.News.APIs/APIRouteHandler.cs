using Abdulrhmaan.News.APIs.UserServices;
using Abdulrhmaan.News.SQlServer;
using Abdulrhmaan.NewsSite.Data;
using Abdulrhmaan.NewsSite.Data.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Abdulrhmaan.News.APIs;

public static class APIRouteHandler
{

    public static void RegisterAuthenticationAPIs(WebApplication app)
    {

        var Logging = app.MapGroup("/newslogging");

        Logging.MapPost("/LogIn", SignIn)
.WithName("LogIn")
.WithOpenApi();

        Logging.MapPost("/LogOut", LogOut)
.WithName("LogOut")
.WithOpenApi().RequireAuthorization();

        Logging.MapPost("/RefreshToken", Refresh)
.WithName("RefreshToken")
.WithOpenApi();


        app.MapPost("/Register", CreateUser)
.WithName("Register")
.WithOpenApi();

        #region News APIs

        app.MapGet("/GetAllNews", GetNews)
.WithName("GetAllNews")
.WithOpenApi();

        app.MapGet("/GetItemById", GetItemById)
.WithName("GetItemById")
.WithOpenApi();

        app.MapGet("/GetNewsByCategoryId", GetNewsByCategoryId)
.WithName("GetNewsByCategoryId")
.WithOpenApi();

        app.MapPost("/AddNews", AddNews)
.WithName("AddNews")
.WithOpenApi();

        app.MapPost("/DeleteNews", DeleteNews)
.WithName("DeleteNews")
.WithOpenApi();

        app.MapPost("/EditNews", EditNews)
.WithName("EditNews")
.WithOpenApi();

        #endregion


        #region Categorys APIs

        app.MapGet("/GetAllCategorys", GetAllCategorys)
.WithName("GetAllCategorys")
.WithOpenApi();

        app.MapGet("/GetCategoryById", GetCategoryById)
.WithName("GetCategoryById")
.WithOpenApi();

        app.MapPost("/AddCategory", AddCategory)
.WithName("AddCategory")
.WithOpenApi();

        app.MapPost("/DeleteCategory", DeleteCategory)
.WithName("DeleteCategory")
.WithOpenApi();

        app.MapPost("/EditCategory", EditCategory)
.WithName("EditCategory")
.WithOpenApi();

        #endregion


    }



    static async Task<IResult> CreateUser([FromBody] RegisterUser RequestUser, [FromServices] IUserService UserService)
    {
        var result = await UserService.RegisterUser(RequestUser);
        var Errors = new List<Error>();
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                var Error = new Error { Code = error.Code, Description = error.Description };
                Errors.Add(Error);
            }
            return TypedResults.BadRequest(Errors);
        }
        return TypedResults.Ok(result.Succeeded);
    }

    //Iam applying soft delete here 
    static async Task<IResult> DeleteNews([FromServices] NewsContext context, int NewsId)
    {
        var NewsToDelete = await context.News.FindAsync(NewsId);

        if (NewsToDelete is not null)
        {
            NewsToDelete.IsDeleted = true;
            context.News.Update(NewsToDelete);
            return TypedResults.NoContent();
        }
        return TypedResults.NotFound("News item not found.");
    }

    static async Task<IResult> EditNews([FromServices] NewsContext context, NewsPostDto NewsDto)
    {
        var NewsToUpdate = await context.News.FindAsync(NewsDto.Id);
        if (NewsToUpdate != null)
        {
            NewsToUpdate.Title = NewsDto.Title;
            NewsToUpdate.Description = NewsDto.Description;
            NewsToUpdate.Status = NewsDto.Status;
            NewsToUpdate.IsDeleted = NewsDto.IsDeleted;
            context.News.Update(NewsToUpdate);
            context.SaveChanges();
            return TypedResults.Ok(NewsToUpdate);
        }
        return TypedResults.NotFound("News item not found.");

    }
    static async Task<IResult> AddNews([FromBody] NewsDto news, [FromServices] NewsContext context)
    {
        var NewsToAdd = new SQlServer.News
        {
            Title = news.Title,
            Description = news.Description,
            Status = news.Status,
            InsertedAt = DateTime.Now,
            UserId = news.UserId,
            CategoryId = news.CategoryId,
            IsDeleted = news.IsDeleted
        };
        await context.News.AddAsync(NewsToAdd);
        await context.SaveChangesAsync();
        return TypedResults.Ok(NewsToAdd);
    }



    static async Task<List<NewsDto>> GetNews([FromServices] NewsContext Context)
    {
        var Items = await Context.News
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.InsertedAt)
            .ToListAsync();
        var dto = Items.Adapt<List<NewsDto>>();
        return dto;
    }

    static async Task<List<NewsDto>> GetNewsByCategoryId([FromServices] NewsContext Context, int CategoryId)
    {
        var Items = await Context.News.Where(x => x.CategoryId == CategoryId).ToListAsync();
        var dto = Items.Adapt<List<NewsDto>>();
        return dto;
    }

    static async Task<IResult> GetItemById([FromServices] NewsContext Context, int ItemId)
    {
        var Items = await Context.News.FindAsync(ItemId);

        if (Items is null)
        {
            return TypedResults.NotFound();
        }
        var NewsDto = Items.Adapt<NewsDto>();
        return TypedResults.Ok(NewsDto);
    }

    static async Task<List<CategoryDto>> GetAllCategorys([FromServices] NewsContext Context)
    {
        var Items = await Context.Categorys.Where(x => x.IsDeleted != true).ToListAsync();
        return Items.Adapt<List<CategoryDto>>();
    }

    static async Task<IResult> AddCategory([FromBody] CategoryPostDto CategoryDto, [FromServices] NewsContext context)
    {
        var CategoryToAdd = new Category
        {
            Name = CategoryDto.Name,
            Description = CategoryDto.Description,
            UserId = CategoryDto.UserId,
            InsertedAt = DateTime.UtcNow,
            Status = CategoryDto.Status,
            IsDeleted = CategoryDto.IsDeleted,
        };
        await context.Categorys.AddAsync(CategoryToAdd);
        await context.SaveChangesAsync();
        return TypedResults.Ok(CategoryToAdd);
    }

    static async Task<IResult> GetCategoryById([FromServices] NewsContext Context, int ItemId)
    {
        var Category = await Context.Categorys.FindAsync(ItemId);

        if (Category is null)
        {
            return TypedResults.NotFound();
        }
        var CategoryDto = Category.Adapt<Category>();
        return TypedResults.Ok(CategoryDto);
    }

    static async Task<IResult> DeleteCategory([FromServices] NewsContext context, int CategoryId)
    {
        var CategoryToDelete = await context.Categorys.FindAsync(CategoryId);

        if (CategoryToDelete is not null)
        {
            CategoryToDelete.IsDeleted = true;
            context.Categorys.Update(CategoryToDelete);
            return TypedResults.NoContent();
        }
        return TypedResults.NotFound("Category Not Found");
    }

    static async Task<IResult> EditCategory([FromServices] NewsContext context, CategoryPostDto CategoryDto)
    {
        var CategoryToUpdate = await context.Categorys.FindAsync(CategoryDto.CategoryId);
        if (CategoryToUpdate is not null)
        {
            CategoryToUpdate.Name = CategoryDto.Name;
            CategoryToUpdate.Description = CategoryDto.Description;
            CategoryToUpdate.Status = CategoryDto.Status;
            CategoryToUpdate.IsDeleted = CategoryDto.IsDeleted;
            context.Categorys.Update(CategoryToUpdate);
            await context.SaveChangesAsync();
            return TypedResults.Ok(CategoryToUpdate);
        }
        return TypedResults.NotFound("Category Not Found");
    }

    static async Task<IResult> SignIn(LoginUser RequestUser, [FromServices] IUserAuthService AuthenticationService, [FromServices] ITokenManager TokenManager)
    {

        if (!await AuthenticationService.ValidateUser(RequestUser))
            return TypedResults.Unauthorized();

        var tokenDto = await AuthenticationService.CreateToken(populateExp: true);
        //whitelisting token into redis
        await TokenManager.SaveAccessToken(tokenDto.AccessToken);
        return TypedResults.Ok(tokenDto);

    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    static async Task<IResult> LogOut([FromServices] ITokenManager TokenManager)
    {
        await TokenManager.DeactivateCurrentAsync();
        return TypedResults.Unauthorized();
    }

    static async Task<IResult> Refresh(TokenDto tokenDto, [FromServices] IUserAuthService AuthenticationService)
    {
        var tokenDtoToReturn = await AuthenticationService.RefreshToken(tokenDto);
        return TypedResults.Ok(tokenDtoToReturn);
    }

}

