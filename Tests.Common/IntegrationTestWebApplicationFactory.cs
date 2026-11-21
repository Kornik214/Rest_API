using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace Tests.Common;

public sealed class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("control_work_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public async Task InitializeAsync() => await _postgres.StartAsync();

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        Dispose();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddJsonFile("appsettings.Test.json", optional: true));
        builder.ConfigureServices(services =>
        {
            services.RemoveServiceByType(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveServiceByType(typeof(ApplicationDbContext));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_postgres.GetConnectionString(), npgsql => npgsql.MigrationsAssembly("Infrastructure"))
                    .UseSnakeCaseNamingConvention());
        });
    }
}

public static class ServiceCollectionExtensions
{
    public static void RemoveServiceByType(this IServiceCollection services, Type serviceType)
    {
        var descriptors = services.Where(descriptor => descriptor.ServiceType == serviceType).ToArray();
        foreach (var descriptor in descriptors)
            services.Remove(descriptor);
    }
}
