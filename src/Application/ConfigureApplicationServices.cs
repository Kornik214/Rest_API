using Application.Scooters.Services.Abstract;
using Application.Scooters.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureApplicationServices
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IScooterService, ScooterService>();
    }
}