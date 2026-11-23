using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ReadBlogpostTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private const string ApiRoute = "/api/blogposts";

    public ReadBlogpostTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    // Test of we alle blogposts succesvol kunnen ophalen zonder in te loggen
    public async Task readBlogposts_ReturnsSuccess()
    {
        // Arrange: Zet een standaard test client klaar
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(ApiRoute);

        // Assert: check of we een succes code krijgen
        response.EnsureSuccessStatusCode();
        
        // Assert: check of de data in json is
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}