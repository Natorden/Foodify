using AuthService.Core.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared;
namespace AuthService.Infrastructure.RpcServices;

public class ProfileRpcService : Profile.ProfileBase {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ProfileRpcService> _logger;
    
    public ProfileRpcService(UserManager<ApplicationUser> userManager, ILogger<ProfileRpcService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
    
    public async override Task<ProfileReturnModel> GetUserProfile(ProfileLookupModel request, ServerCallContext context)
    {
        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User with id {request.Id} not found"));
        }
        
        var profileReturnModel = new ProfileReturnModel {
            Id = user.Id.ToString(),
            DisplayName = user.DisplayName ?? "",
            UserName = user.UserName,
            ProfilePicturePath = user.ProfilePicturePath ?? ""
        };
        return profileReturnModel;
    }

    public async override Task<ProfileMultiReturnModel> GetManyUserProfiles(ProfileMultiLookupModel request, ServerCallContext context)
    {
        var response = new ProfileMultiReturnModel();
        var users = await _userManager.Users
            .Where(user => request.Ids.Contains(user.Id.ToString()))
            .Select(user => new ProfileReturnModel {
                Id = user.Id.ToString(),
                DisplayName = user.DisplayName ?? "",
                UserName = user.UserName,
                ProfilePicturePath = user.ProfilePicturePath ?? ""
            })
            .ToListAsync();
        response.Profiles.AddRange(users);
        
        return response;
    }
}
