using AuthService.Core.Models;
using AuthService.Core.Models.BindingModels;
using AuthService.Core.Models.Dto;
using AuthService.Core.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace AuthService.Controllers;

/// <summary>
/// Controller responsible for user profiles
/// </summary>
[Route("/Identity/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves the profile of the authenticated user.
    /// </summary>
    /// <returns>An IActionResult containing the user's profile info.</returns>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(new UserProfileDto(user!));
    }

    /// <summary>
    /// Retrieves the profile of a user based on their ID.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>An IActionResult containing the user's profile info.</returns>
    /// <exception cref="NotFoundException">Thrown if the user is not found.</exception>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProfileById(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) {
            throw new NotFoundException("User not found");
        }
        return Ok(new UserProfileDto(user));
    }

    /// <summary>
    /// Updates the profile of the authenticated user.
    /// </summary>
    /// <param name="model">The binding model containing the updated profile information.</param>
    /// <returns>An IActionResult indicating the result of the profile update.</returns>
    /// <exception cref="NotFoundException">Thrown if the user is not found.</exception>
    /// <exception cref="BadRequestException">Thrown if the profile update fails.</exception>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(ProfilePutBindingModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            throw new NotFoundException("User not found");
        }
        user.DisplayName = model.DisplayName;
        user.ProfilePicturePath = model.ProfilePicturePath;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded) {
            return Ok(user);
        }
        throw new BadRequestException("Failed to update user profile");
    }
}
