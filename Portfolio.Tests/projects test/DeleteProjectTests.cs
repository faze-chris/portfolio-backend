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

public class DeleteProjectTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string ApiRoute = "/api/projects";
    private const string AuthSchemeName = "MockAuth";
    private const string RoleName = "Admin";
    private const string MockUserName = "FakeAdmin";

    public DeleteProjectTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

     // Dit is een mock login handler voor het testen van de delete API 
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
    // Test of een user zonder rol een project kan verwijderen
    public async Task deleteProject_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange: Zet een standaard user klaar
        var client = _factory.CreateClient();
        string expectedErrorPrefix = "Verwachtte 401 of 403, maar kreeg ";

        // Act
        var response = await client.DeleteAsync(ApiRoute + "/1");

        // Assert: check of error code krijgen
        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden,
            expectedErrorPrefix + response.StatusCode
        );
    }

    [Fact]
    // Test of een Admin wel een project kan verwijderen
    public async Task deleteProject_AsAdmin_ReturnsNoContent()
    {
        // Arrange: maak een admin user aan en maak eerst een project om daarna te verwijderen
        var client = _factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddAuthentication(AuthSchemeName).AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthSchemeName, _ => { })
        )).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthSchemeName);

        // data voor een tijdelijk project
        var tempProject = new
        {
            Title = "Wegermee Project",
            Description = "Description",
            Category = "Web",
            GitHubUrl = "url",
            Date = DateTime.UtcNow,
            ImageUrl = "img"
        };

        // Maak het project aan en haal de URL op
        var postResponse = await client.PostAsJsonAsync(ApiRoute, tempProject);
        var location = postResponse.Headers.Location?.ToString();

        // Act: stuur het delete verzoek naar de specifieke URL van het nieuwe temp project
        var response = await client.DeleteAsync(location);

        // Assert: Controleer of de server de status 204 code
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}