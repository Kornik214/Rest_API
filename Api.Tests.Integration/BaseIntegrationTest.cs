using System.Net.Http.Headers;
using Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebApplicationFactory>
{
    protected readonly IntegrationTestWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.WithWebBuilderMock().CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.Scheme);
    }

    protected IServiceScope CreateScope() => Factory.Services.CreateScope();
}
