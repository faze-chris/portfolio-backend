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

public class DeleteBlogpostTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string ApiRoute = "/api/blogposts";
    private const string AuthSchemeName = "MockAuth";
    private const string RoleName = "Admin";
    private const string MockUserName = "FakeAdmin";

    public DeleteBlogpostTests(WebApplicationFactory<Program> factory)
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
    // Test of een user zonder rol een blogpost kan verwijderen
    public async Task deleteBlogpost_WithoutAuth_ReturnsUnauthorized()
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
    // Test of een Admin wel een blogpost kan verwijderen
    public async Task deleteBlogpost_AsAdmin_ReturnsNoContent()
    {
        // Arrange: maak een admin user aan via mock
        var client = _factory.WithWebHostBuilder(b => b.ConfigureServices(s =>
            s.AddAuthentication(AuthSchemeName).AddScheme<AuthenticationSchemeOptions, MockAuthHandler>(AuthSchemeName, _ => { })
        )).CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthSchemeName);

        // data voor tijdelijke blogpost
        var tempBlogpost = new
        {
            Title = "Wegermee Blogpost",
            Content = "Content",
            Date = DateTime.UtcNow,
            ImageUrl = "img",
            LinkedInUrl = "url"
        };

        // Maak het blogpost aan en haal de URL op
        var postResponse = await client.PostAsJsonAsync(ApiRoute, tempBlogpost);
        var location = postResponse.Headers.Location?.ToString();

        // Act
        var response = await client.DeleteAsync(location);

        // Assert: Controleer of de server de status 204 code stuurt
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}