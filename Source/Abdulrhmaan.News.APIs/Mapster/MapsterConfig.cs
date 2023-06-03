using Abdulrhmaan.News.SQlServer;
using Abdulrhmaan.NewsSite.Data;
using Mapster;

namespace Abdulrhmaan.News.APIs
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig<User, RegisterUser>.NewConfig().TwoWays().PreserveReference(false);
            TypeAdapterConfig<User, LoginUser>.NewConfig().PreserveReference(false);

            TypeAdapterConfig<Category, CategoryDto>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.InsertedAt, src => src.InsertedAt)
                .Map(dest => dest.IsDeleted, src => src.IsDeleted)
                .Map(dest => dest.Status, src => src.Status)
                .Map(dest => dest.CategoryId, src => src.Id)
                .TwoWays().PreserveReference(false);

            TypeAdapterConfig<SQlServer.News, NewsDto>.NewConfig()
    .Map(dest => dest.Id, src => src.Id)
    .Map(dest => dest.Description, src => src.Description)
    .Map(dest => dest.InsertedAt, src => src.InsertedAt)
    .Map(dest => dest.IsDeleted, src => src.IsDeleted)
    .Map(dest => dest.Status, src => src.Status)
    .Map(dest => dest.CategoryId, src => src.Id)
    .Map(dest => dest.UserId, src => src.UserId)
     .Map(dest => dest.AuthorName, src => src.User.Name)
     .Map(dest => dest.CategoryName, src => src.Category.Name)

    .TwoWays().PreserveReference(false);
        }
    }
}
