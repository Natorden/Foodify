using CommentService.Core.Interfaces;
using CommentService.Infrastructure.Interfaces;
using CommentService.Infrastructure.Repositories;
namespace CommentService.Extensions;

public static class ServicesAndRepositoryExtension
{
    public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
    {

        #region Service
        
        services.AddTransient<ICommentService, Core.Services.CommentService>();
        
        #endregion

        #region Repository
        
        services.AddTransient<ICommentRepository, CommentRepository>();
        
        #endregion

        return services;
    }
}
