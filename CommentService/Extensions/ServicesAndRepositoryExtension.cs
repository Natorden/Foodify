using CommentService.Core.Context;
using CommentService.Core.Interfaces;
using CommentService.Infrastructure.Interfaces;
using CommentService.Infrastructure.Repositories;
using CommentService.Infrastructure.RpcClients;
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

        #region gRPC
        
        services.AddTransient<IProfileRpcClient, ProfileRpcClient>();
        
        #endregion
        
        services.AddScoped<CurrentContext>();

        return services;
    }
}
