using System.Security.Claims;
using System.Security.Principal;
namespace CommentService.Core.Context;

public class CurrentContext
{
    public HttpContext HttpContext { get; private set; } = null!;
    public Guid? UserId { get; private set; }
    public List<string>? Roles { get; private set; }

    public void Build(HttpContext httpContext)
    {
        HttpContext = httpContext;

        SetUser(httpContext.User);
    }

    private void SetUser(IPrincipal user)
    {
        if (user.Identity is ClaimsIdentity identity)
        {
            if (identity.Claims.Any())
            {
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                UserId = userId is null
                    ? null
                    : Guid.Parse(userId);
                Roles = identity.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
            }
        }
    }
}
