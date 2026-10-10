using Application.Common.Interfaces;                 // IProductRepository
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistenceServices
{
    public static void AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ----------- рядок підключення -------------
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // NpgsqlDataSource дозволяє включити додаткові можливості (наприклад, dynamic JSON)
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();    // якщо плануєте працювати з jsonb
        var dataSource = dataSourceBuilder.Build();

        // ---------- реєстрація DbContext ----------
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                    dataSource,
                    npgsqlOptions =>
                        npgsqlOptions.MigrationsAssembly(
                            typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention() // snake_case у PostgreSQL
                .ConfigureWarnings(warnings =>
                {
                    warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
                }));

        // ---------- ініціалізатор бази -------------
        services.AddScoped<ApplicationDbContextInitialiser>();

        // ---------- реєстрація репозиторіїв -------
        services.AddRepositories();
    }

    // -------------------------------------------------
    private static void AddRepositories(this IServiceCollection services)
    {
        // Реєструємо конкретну реалізацію і інтерфейс одночасно.
        services.AddScoped<IScooterRepository, ScooterRepository>();
    }
}
