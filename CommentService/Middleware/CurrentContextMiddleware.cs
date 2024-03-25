using CommentService.Core.Context;
namespace CommentService.Middleware;

public class CurrentContextMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext, CurrentContext currentContext)
    {
        currentContext.Build(httpContext);
        return _next.Invoke(httpContext);
    }
}
