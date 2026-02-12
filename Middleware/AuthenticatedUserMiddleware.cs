using System.Security.Claims;
using VH_2ND_TASK.Middleware.Model;

namespace VH_2ND_TASK.Middleware;

    public class AuthenticatedUserMiddleware(ICurrentUser currentUser) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var idValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(idValue) && int.TryParse(idValue, out var id))
            {
                currentUser.UserId = id;
                currentUser.IsAuthenticated = true;
            }
            else
            {
                currentUser.UserId = null;
                currentUser.IsAuthenticated = false;
            }
        }
        else
        {
            currentUser.UserId = null;
            currentUser.IsAuthenticated = false;
        }
        await next(context);
        }
    }

