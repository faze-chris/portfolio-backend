using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostProject_WithoutAuthentication_ReturnsUnauthorizedOrForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act 
        var response = await client.PostAsync("/api/projects", null);

        // Assert 
        Assert.True(
            response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
            response.StatusCode == System.Net.HttpStatusCode.Forbidden,
            $"Verwachtte Unauthorized of Forbidden, maar kreeg {response.StatusCode}"
        );
    }
}