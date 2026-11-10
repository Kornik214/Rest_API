using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Scooters;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data.Scooters;
using Xunit;

namespace Api.Tests.Integration.Scooters;

public class ScootersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Scooter _firstTestScooter = ScootersTestData.FirstTestScooter();
    private readonly Scooter _secondTestScooter = ScootersTestData.SecondTestScooter();
    private readonly Scooter _thirdTestScooter = ScootersTestData.ThirdTestScooter();
    private const string ScootersRoute = "scooters";

    public ScootersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    public async Task InitializeAsync()
    {
        await Context.Scooters.AddAsync(_secondTestScooter);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Scooters.RemoveRange(Context.Scooters);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task ShouldCreateScooter()
    {
        var response = await Client.PostAsJsonAsync(ScootersRoute, CreateRequest(_firstTestScooter));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseDto = await response.Content.ReadFromJsonAsync<ScooterDto>();
        responseDto.Should().NotBeNull();
        var dbScooter = await Context.Scooters.FindAsync(new ScooterId(responseDto!.Id));
        dbScooter.Should().NotBeNull();
        dbScooter!.Model.Should().Be(_firstTestScooter.Model);
        dbScooter.SerialNumber.Should().Be(_firstTestScooter.SerialNumber);
    }

    [Fact]
    public async Task ShouldNotCreateScooterBecauseSerialNumberAlreadyTaken()
    {
        var response = await Client.PostAsJsonAsync(ScootersRoute, CreateRequest(_secondTestScooter));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var matchingScooters = await Context.Scooters
            .Where(scooter => scooter.SerialNumber == _secondTestScooter.SerialNumber)
            .ToListAsync();
        matchingScooters.Should().ContainSingle();
        matchingScooters[0].Id.Should().Be(_secondTestScooter.Id);
    }

    [Fact]
    public async Task ShouldNotCreateScooterBecauseRequestIsInvalid()
    {
        var request = new CreateScooterDto("", "", 101, 0m, true, default);

        var response = await Client.PostAsJsonAsync(ScootersRoute, request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await Context.Scooters.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ShouldUpdateScooter()
    {
        var response = await Client.PutAsJsonAsync(RouteFor(_secondTestScooter.Id), UpdateRequest(_thirdTestScooter));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseDto = await response.Content.ReadFromJsonAsync<ScooterDto>();
        responseDto!.Id.Should().Be(_secondTestScooter.Id.Value);
        var dbScooter = await Context.Scooters.FindAsync(_secondTestScooter.Id);
        dbScooter!.Model.Should().Be(_thirdTestScooter.Model);
        dbScooter.SerialNumber.Should().Be(_thirdTestScooter.SerialNumber);
        dbScooter.PricePerMinute.Should().Be(_thirdTestScooter.PricePerMinute);
    }

    [Fact]
    public async Task ShouldNotUpdateScooterBecauseSerialNumberAlreadyTaken()
    {
        await Context.Scooters.AddAsync(_firstTestScooter);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var response = await Client.PutAsJsonAsync(RouteFor(_secondTestScooter.Id), UpdateRequest(_firstTestScooter));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var scooters = await Context.Scooters.OrderBy(scooter => scooter.SerialNumber).ToListAsync();
        scooters.Should().HaveCount(2);
        scooters.Single(scooter => scooter.Id == _secondTestScooter.Id).SerialNumber
            .Should().Be(_secondTestScooter.SerialNumber);
        scooters.Single(scooter => scooter.Id == _firstTestScooter.Id).SerialNumber
            .Should().Be(_firstTestScooter.SerialNumber);
    }

    [Fact]
    public async Task ShouldNotUpdateScooterBecauseScooterNotFound()
    {
        var response = await Client.PutAsJsonAsync(RouteFor(ScooterId.New()), UpdateRequest(_thirdTestScooter));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await Context.Scooters.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ShouldDeleteScooter()
    {
        var response = await Client.DeleteAsync(RouteFor(_secondTestScooter.Id));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await Context.Scooters.FindAsync(_secondTestScooter.Id)).Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteScooterBecauseScooterNotFound()
    {
        var response = await Client.DeleteAsync(RouteFor(ScooterId.New()));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await Context.Scooters.FindAsync(_secondTestScooter.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldGetScooter()
    {
        var response = await Client.GetAsync(RouteFor(_secondTestScooter.Id));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseDto = await response.Content.ReadFromJsonAsync<ScooterDto>();
        responseDto!.Id.Should().Be(_secondTestScooter.Id.Value);
    }

    [Fact]
    public async Task ShouldGetAllScooters()
    {
        var response = await Client.GetAsync(ScootersRoute);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var scooters = await response.Content.ReadFromJsonAsync<List<ScooterDto>>();
        scooters.Should().ContainSingle(scooter => scooter.Id == _secondTestScooter.Id.Value);
    }

    private static CreateScooterDto CreateRequest(Scooter scooter) => new(
        scooter.Model, scooter.SerialNumber, scooter.BatteryPercent, scooter.PricePerMinute,
        scooter.IsAvailable, scooter.LastServiceDate);

    private static UpdateScooterDto UpdateRequest(Scooter scooter) => new(
        scooter.Model, scooter.SerialNumber, scooter.BatteryPercent, scooter.PricePerMinute,
        scooter.IsAvailable, scooter.LastServiceDate);

    private static string RouteFor(ScooterId id) => $"{ScootersRoute}/{id}";
}
