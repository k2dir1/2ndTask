using Microsoft.Extensions.DependencyInjection;
using VH_2ND_TASK.Application.Abstractions.Auth;
using VH_2ND_TASK.Application.Abstractions.Books;
using VH_2ND_TASK.Application.Services;

namespace VH_2ND_TASK.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}