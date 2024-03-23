using CommentService.Infrastructure.Interfaces;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.Models;
namespace CommentService.Infrastructure.RpcClients;

public class ProfileRpcClient : IProfileRpcClient {
    private readonly Profile.ProfileClient _profileClient;
    public ProfileRpcClient(Profile.ProfileClient profileClient)
    {
        _profileClient = profileClient;
    }

    public async Task<SharedUserProfileDto?> GetUserProfileById(Guid id)
    {
        try {
            var response = await _profileClient.GetUserProfileAsync(
                new ProfileLookupModel {
                    Id = id.ToString()
                }
            );
            return response != null ? new SharedUserProfileDto {
                Id = Guid.Parse(response.Id),
                DisplayName = response.DisplayName == "" ? null : response.DisplayName,
                UserName = response.UserName,
                ProfilePicturePath = response.ProfilePicturePath == "" ? null : response.ProfilePicturePath
            } : null;
        } catch (RpcException) {
            Console.WriteLine("Profile service unavailable");
            return null;
        }
    }
    public async Task<List<SharedUserProfileDto>?> GetUserProfilesByIds(List<Guid> ids)
    {
        var request = new ProfileMultiLookupModel();
        request.Ids.AddRange(ids.Select(id => id.ToString()));
        var response = await _profileClient.GetManyUserProfilesAsync(request);
        
        return response?.Profiles.Select(profile => new SharedUserProfileDto {
                Id = Guid.Parse(profile.Id),
                DisplayName = profile.DisplayName == "" ? null : profile.DisplayName,
                UserName = profile.UserName,
                ProfilePicturePath = profile.ProfilePicturePath == "" ? null : profile.ProfilePicturePath
            })
            .ToList();
    }
}
