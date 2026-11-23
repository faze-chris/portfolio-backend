using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

public class UpdateProjectTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string ApiRoute = "/api/projects";
    private const string AuthSchemeName = "MockAuth";
    private const string RoleName = "Admin";
    private const string MockUserName = "FakeAdmin";

    public UpdateProjectTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    // Dit is een mock login handler voor het testen van de update API
    private class MockAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public MockAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // fake user met de rol admin
            var claims = new[] { new Claim(ClaimTypes.Name, MockUserName), new Claim(ClaimTypes.Role, RoleName) };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, AuthSchemeName)), AuthSchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    [Fact]
    // Test of een user zonder rol een project kan updaten
    public async Task updateProject_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange: Zet een standaard user en testdata klaar
        var client = _factory.CreateClient();
        var updateData = new { Title = "randum Update" };
        string expectedErrorPrefix = "Verwachtte 401 of 403, maar kreeg ";

        // Act
        var response = await client.PutAsJsonAsync(ApiRoute + "/1", updateData);

        // Assert: check of error code krijgen
        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
            expectedErrorPrefix + response.StatusCode
        );
    }

    [Fact]
    // Test of een Admin wel een bestaand project kan updaten
    public async Task updateProject_AsAdmin_ReturnsNoContent()
    {
        // Arrange: maak een admin user aan via onze mock
        var client = _factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddAuthentication(AuthSchemeName).AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthSchemeName, _ => { })
        )).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthSchemeName);

        // data voor nieuwe project om eerst aan te maken
        var initialProject = new
        {
            Title = "Oud Project",
            Description = "Oude Description",
            Category = "Web",
            GitHubUrl = "url",
            Date = DateTime.UtcNow,
            ImageUrl = "img"
        };

        // Maak het project aan create (POST) en haal de URL op
        var postResponse = await client.PostAsJsonAsync(ApiRoute, initialProject);
        var location = postResponse.Headers.Location?.ToString();

        // data voor update PUT
        var updatedProject = new
        {
            Title = "UPDATE Project",
            Description = "UPDATE Description",
            Category = "Web",
            GitHubUrl = "url",
            Date = DateTime.UtcNow,
            ImageUrl = "img"
        };

        // Act
        var response = await client.PutAsJsonAsync(location, updatedProject);

        // Assert: Controleer of de server de status 204 
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}