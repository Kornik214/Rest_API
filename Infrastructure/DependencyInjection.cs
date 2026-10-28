using Application.Scooters;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();
        });
        
        services.AddScoped<ScooterRepository>();
        services.AddScoped<IScooterRepository>(sp => sp.GetRequiredService<ScooterRepository>());
        services.AddScoped<IScooterQueries>(sp => sp.GetRequiredService<ScooterRepository>());

        services.AddTransient<DatabaseInitializer>();

        return services;
    }
}