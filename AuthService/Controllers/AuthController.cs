using AuthService.Configuration;
using AuthService.Core.Context;
using AuthService.Core.Models;
using AuthService.Core.Models.Dto;
using AuthService.Core.Models.Exceptions;
using AuthService.Core.Models.Responses;
using AuthService.Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace AuthService.Controllers;

/// <summary>
/// Controller responsible for user authentication
/// </summary>
[Route("/Identity/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly CurrentContext _currentContext;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService, IOptions<JwtSettings> jwtSettings, CurrentContext currentContext)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings;
        _currentContext = currentContext;
    }

    /// <summary>
    /// Endpoint for user login to authenticate and generate a JWT token.
    /// </summary>
    /// <param name="request">The login request containing the user's email and password.</param>
    /// <returns>An IActionResult representing the HTTP response.</returns>
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u 
            => u.NormalizedUserName! == request.EmailOrUserName.ToUpper() 
            || u.NormalizedEmail!.ToLower() == request.EmailOrUserName.ToUpper());
        if (user == null)
        {
            throw new AuthException("Wrong username or password");
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            throw new AuthException("Wrong username or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateJwtToken(user, roles, null);
        
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);
        
        return Ok(new AuthResponse
        {
            Email = user.Email!,
            UserId = user.Id,
            Token = token,
            RefreshToken = refreshToken.Token,
            RefreshExpiresAt = refreshToken.ExpiresAt,
            Roles = roles.ToList()
        });
    }
    
    /// <summary>
    /// Endpoint for user login to generate a JWT token based on a refresh token.
    /// </summary>
    /// <param name="request">The refresh issued ot the user on login.</param>
    /// <returns>An IActionResult representing the HTTP response.</returns>
    [HttpPost("Refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    public async Task<IActionResult> Login([FromBody] RefreshRequestDto request)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == request.UserId);
        if (user == null)
        {
            throw new AuthException("User not found");
        }
        var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
        if (refreshToken is null || !refreshToken.IsActive) 
        {
            throw new AuthException("Invalid refresh token");
        }
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateJwtToken(user, roles, null);
        
        // Revoke the used token, and keep it for archival purposes
        refreshToken.RevokedAt = DateTime.UtcNow;
        
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);
        
        return Ok(new AuthResponse
        {
            Email = user.Email!,
            UserId = user.Id,
            Token = token,
            RefreshToken = newRefreshToken.Token,
            RefreshExpiresAt = newRefreshToken.ExpiresAt,
            Roles = roles.ToList()
        });
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="requestDto">The register request data including email and password.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>An IActionResult representing the result of the registration.</returns>
    /// <exception cref="AuthException">Thrown when there is an error during the registration process.</exception>
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = requestDto.Email,
            DisplayName = requestDto.UserName,
            UserName = requestDto.UserName.ToLower()
        };
        IdentityResult result;
        try
        {
            result = await _userManager.CreateAsync(user, requestDto.Password);
        } catch (DbUpdateException)
        {
            throw new AuthException("Cannot create user. This username is already taken.");
        }
        if (!result.Succeeded)
        {
            var identityError = result.Errors.FirstOrDefault();

            if (identityError == null)
            {
                throw new AuthException("Unknown error occurred while creating user.");
            }

            if (identityError.Code == "DuplicateUserName")
            {
                throw new AuthException("Cannot create user. This username is already taken.");
            }
            throw new AuthException(identityError.Description);
        }

        await _userManager.UpdateAsync(user);

        return Ok();
    }

    /// <summary>
    /// Check if username is taken
    /// </summary>
    /// <param name="username">Username to lookup</param>
    /// <returns></returns>
    [HttpGet("username-taken/{username}")]
    public async Task<IActionResult> IsUsernameTaken([FromRoute] string username)
    {
        var exists = await _userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpper());
        return Ok(exists);
    }
}
