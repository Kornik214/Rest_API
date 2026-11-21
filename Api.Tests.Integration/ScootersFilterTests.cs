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

public sealed class ScootersFilterTests : BaseIntegrationTest, IAsyncLifetime
{
    private List<Domain.Scooters.Scooter> _scooters = [];

    public ScootersFilterTests(IntegrationTestWebApplicationFactory factory) : base(factory) { }

    public async Task InitializeAsync()
    {
        using var scope = CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE scooters");
        _scooters = [ScootersTestData.First(), ScootersTestData.Second(), ScootersTestData.Third(), ScootersTestData.Fourth()];
        context.Scooters.AddRange(_scooters);
        await context.SaveChangesAsync();
        _scooters = await context.Scooters.OrderBy(s => s.SerialNumber).ToListAsync();
        context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        using var scope = CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE scooters");
        context.ChangeTracker.Clear();
    }

    [Fact] public Task Filter_by_serial_number_is_case_insensitive() => AssertIds(new("alpha", null, null, null, null, null, null, null), "KIV-ALPHA-001");
    [Fact] public Task Filter_by_battery_from() => AssertIds(new(null, 70, null, null, null, null, null, null), "KIV-ALPHA-001", "KIV-BETA-002");
    [Fact] public Task Filter_by_battery_to() => AssertIds(new(null, null, 40, null, null, null, null, null), "LVIV-GAMMA-003", "ODESA-DELTA-004");
    [Fact] public Task Filter_by_availability() => AssertIds(new(null, null, null, true, null, null, null, null), "KIV-ALPHA-001", "LVIV-GAMMA-003");
    [Fact] public Task Filter_by_maintenance_from() => AssertIds(new(null, null, null, null, Utc(2026, 3, 15), null, null, null), "KIV-BETA-002", "LVIV-GAMMA-003", "ODESA-DELTA-004");
    [Fact] public Task Filter_by_maintenance_to() => AssertIds(new(null, null, null, null, null, Utc(2026, 3, 15), null, null), "KIV-ALPHA-001", "KIV-BETA-002");

    [Fact]
    public async Task Filter_by_created_at_from_and_to_uses_database_values()
    {
        var ordered = _scooters.OrderBy(s => s.CreatedAt).ToArray();
        await AssertIds(new(null, null, null, null, null, null, ordered[1].CreatedAt.AddMilliseconds(-1), null), ordered[1].SerialNumber, ordered[2].SerialNumber, ordered[3].SerialNumber);
        await AssertIds(new(null, null, null, null, null, null, null, ordered[2].CreatedAt.AddMilliseconds(1)), ordered[0].SerialNumber, ordered[1].SerialNumber, ordered[2].SerialNumber);
    }

    [Fact]
    public Task Filter_ranges_are_inclusive() => AssertIds(new(null, 40, 70, null, Utc(2026, 3, 15), Utc(2026, 6, 20), null, null), "KIV-BETA-002", "LVIV-GAMMA-003");

    [Fact]
    public Task Filter_combines_conditions_with_and() => AssertIds(new("kiv", 80, 100, true, null, null, null, null), "KIV-ALPHA-001");

    [Fact]
    public Task Filter_without_parameters_returns_every_scooter() => AssertIds(new(null, null, null, null, null, null, null, null), _scooters.Select(s => s.SerialNumber).ToArray());

    [Fact]
    public async Task Filter_with_no_matches_returns_ok_and_empty_list()
    {
        var response = await Client.PostAsJsonAsync("/api/scooters/filter", new SearchScootersRequest("missing", null, null, null, null, null, null, null));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<ScooterResponse>>())!.Should().BeEmpty();
    }

    private async Task AssertIds(SearchScootersRequest request, params string[] expectedSerialNumbers)
    {
        var response = await Client.PostAsJsonAsync("/api/scooters/filter", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var actual = await response.Content.ReadFromJsonAsync<List<ScooterResponse>>();
        actual.Should().NotBeNull();
        actual!.Select(s => s.Id).Should().BeEquivalentTo(
            _scooters.Where(s => expectedSerialNumbers.Contains(s.SerialNumber)).Select(s => s.Id.Value));
    }

    private static DateTime Utc(int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Utc);
}
