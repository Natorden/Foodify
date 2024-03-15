using AuthService.Core.Models;
using AuthService.Core.Models.BindingModels;
using AuthService.Core.Models.Dto;
using AuthService.Core.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace AuthService.Controllers;

/// <summary>
/// Controller responsible for user authentication
/// </summary>
[Route("/Identity/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(user);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProfileById(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) {
            throw new NotFoundException("User not found");
        }
        return Ok(new UserProfileDto(user));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(ProfilePutBindingModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) {
            return NotFound();
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
