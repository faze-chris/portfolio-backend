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

public class CreateProjectTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string ApiRoute = "/api/projects";
    private const string AuthSchemeName = "Test";
    private const string RoleName = "Admin";
    private const string TestUserName = "TestAdmin";

    public CreateProjectTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    //dit is voor het testen van de login api zo we kunnen testen of een ingelogde Admin een project kan aanmaken
    private class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // fake user met de rol admin
            var claims = new[] { new Claim(ClaimTypes.Name, TestUserName), new Claim(ClaimTypes.Role, RoleName) };
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, AuthSchemeName)), AuthSchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    [Fact]
    // Test of een user zonder rol een project kan maken
    public async Task createProject_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange: Zet een standaard user en testdata klaar
        var client = _factory.CreateClient();
        var newProject = new { Title = "randum Project" };
        string expectedErrorPrefix = "Verwachtte 401 of 403, maar kreeg ";

        // Act
        var response = await client.PostAsJsonAsync(ApiRoute, newProject);

        // Assert: check of error code krijgen
        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
            expectedErrorPrefix + response.StatusCode
        );
    }

    [Fact]
    // Test of een Admin wel een project kan aanmaken
    public async Task createProject_AsAdmin_ReturnsCreated()
    {
        // Arrange: maak een admin user aan met testdata
        var client = _factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddAuthentication(AuthSchemeName).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(AuthSchemeName, _ => { })
        )).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthSchemeName);

        // data voor nieuwe project
        var newProject = new
        {
            Title = "Geldig Project",
            Description = "Description",
            Category = "Web",
            GitHubUrl = "url",
            Date = DateTime.UtcNow,
            ImageUrl = "img"
        };

        // Act
        var response = await client.PostAsJsonAsync(ApiRoute, newProject);

        // Assert: Controleer of de server de status 201 code stuurt
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}