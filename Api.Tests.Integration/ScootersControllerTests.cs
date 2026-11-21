using System.Net;
using System.Net.Http.Json;
using Api.Contracts;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public sealed class ScootersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    public ScootersControllerTests(IntegrationTestWebApplicationFactory factory) : base(factory) { }

    public async Task InitializeAsync()
    {
        using var scope = CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE scooters");
        context.Scooters.AddRange(ScootersTestData.First(), ScootersTestData.Second());
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        using var scope = CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE scooters");
        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Create_with_valid_data_returns_created_and_persists_scooter()
    {
        var request = new CreateScooterRequest("DNIPRO-NEW-005", 88, true, Utc(2026, 10, 1));
        var response = await Client.PostAsJsonAsync("/api/scooters", request);
        var created = await response.Content.ReadFromJsonAsync<ScooterResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        created!.Id.Should().NotBeEmpty();
        created.SerialNumber.Should().Be(request.SerialNumber);
        using var scope = CreateScope();
        (await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Scooters.FindAsync(created.Id))!.BatteryLevel.Should().Be(88);
    }

    [Fact]
    public async Task Create_with_duplicate_serial_number_returns_conflict_and_preserves_one_record()
    {
        var response = await Client.PostAsJsonAsync("/api/scooters", new CreateScooterRequest("KIV-ALPHA-001", 80, true, Utc(2026, 1, 1)));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        using var scope = CreateScope();
        (await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Scooters.CountAsync(s => s.SerialNumber == "KIV-ALPHA-001")).Should().Be(1);
    }

    [Fact]
    public async Task Create_with_invalid_data_returns_bad_request_and_does_not_persist()
    {
        var response = await Client.PostAsJsonAsync("/api/scooters", new CreateScooterRequest("", 101, true, DateTime.UtcNow));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        using var scope = CreateScope();
        (await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Scooters.CountAsync()).Should().Be(2);
    }

    [Fact]
    public async Task Update_changes_all_fields()
    {
        var scooter = await GetScooterAsync("KIV-ALPHA-001");
        var request = new UpdateScooterRequest("KIV-ALPHA-UPDATED", 20, false, Utc(2026, 11, 1));
        var response = await Client.PutAsJsonAsync($"/api/scooters/{scooter.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var scope = CreateScope();
        var stored = await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Scooters.FindAsync(scooter.Id);
        stored!.SerialNumber.Should().Be(request.SerialNumber);
        stored.BatteryLevel.Should().Be(20);
        stored.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Update_with_own_serial_number_succeeds()
    {
        var scooter = await GetScooterAsync("KIV-ALPHA-001");
        var response = await Client.PutAsJsonAsync($"/api/scooters/{scooter.Id}", new UpdateScooterRequest("KIV-ALPHA-001", 90, true, Utc(2026, 1, 10)));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_with_other_scooter_serial_returns_conflict_without_changes()
    {
        var scooter = await GetScooterAsync("KIV-ALPHA-001");
        var response = await Client.PutAsJsonAsync($"/api/scooters/{scooter.Id}", new UpdateScooterRequest("KIV-BETA-002", 20, false, Utc(2026, 2, 1)));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        (await GetScooterAsync("KIV-ALPHA-001")).BatteryLevel.Should().Be(95);
    }

    [Fact]
    public async Task Update_nonexistent_scooter_returns_not_found()
    {
        var response = await Client.PutAsJsonAsync($"/api/scooters/{Guid.NewGuid()}", new UpdateScooterRequest("NEW-001", 20, false, Utc(2026, 2, 1)));
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_with_invalid_data_returns_bad_request_without_changes()
    {
        var scooter = await GetScooterAsync("KIV-ALPHA-001");
        var response = await Client.PutAsJsonAsync($"/api/scooters/{scooter.Id}", new UpdateScooterRequest("", -1, false, Utc(2026, 2, 1)));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await GetScooterAsync("KIV-ALPHA-001")).BatteryLevel.Should().Be(95);
    }

    [Fact]
    public async Task Get_endpoints_return_existing_records_and_not_found_for_unknown_id()
    {
        var all = await Client.GetAsync("/api/scooters");
        var scooter = await GetScooterAsync("KIV-ALPHA-001");
        var one = await Client.GetAsync($"/api/scooters/{scooter.Id}");
        var missing = await Client.GetAsync($"/api/scooters/{Guid.NewGuid()}");

        (await all.Content.ReadFromJsonAsync<List<ScooterResponse>>())!.Select(s => s.Id).Should().Contain(scooter.Id.Value);
        one.StatusCode.Should().Be(HttpStatusCode.OK);
        missing.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Domain.Scooters.Scooter> GetScooterAsync(string serialNumber)
    {
        using var scope = CreateScope();
        return (await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Scooters.SingleAsync(s => s.SerialNumber == serialNumber));
    }

    private static DateTime Utc(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Utc);
}
