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

public class UpdateBlogpostTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string ApiRoute = "/api/blogposts";
    private const string AuthSchemeName = "MockAuth";
    private const string RoleName = "Admin";
    private const string MockUserName = "FakeAdmin";

    public UpdateBlogpostTests(WebApplicationFactory<Program> factory)
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
    // Test of een user zonder rol een blogpost kan updaten
    public async Task updateBlogpost_WithoutAuth_ReturnsUnauthorized()
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
    // Test of een Admin wel een bestaand blogpost kan updaten
    public async Task updateBlogpost_AsAdmin_ReturnsNoContent()
    {
        // Arrange: maak een admin user aan via mock
        var client = _factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddAuthentication(AuthSchemeName).AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthSchemeName, _ => { })
        )).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthSchemeName);

        // data voor nieuwe blogpost om eerst aan te maken
        var initialBlogpost = new
        {
            Title = "Oud Blogpost",
            Content = "Oude Content",
            Date = DateTime.UtcNow,
            ImageUrl = "img",
            LinkedInUrl = "url"
        };

        // Maak het blogpost aan en haal de URL op
        var postResponse = await client.PostAsJsonAsync(ApiRoute, initialBlogpost);
        var location = postResponse.Headers.Location?.ToString();

        // data voor update
        var updatedBlogpost = new
        {
            Title = "Nieuw Blogpost",
            Content = "Nieuwe Content",
            Date = DateTime.UtcNow,
            ImageUrl = "img",
            LinkedInUrl = "url"
        };

        // Act
        var response = await client.PutAsJsonAsync(location, updatedBlogpost);

        // Assert: Controleer of de server de status 204 code stuurt
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}