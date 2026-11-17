using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ReadProjectTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    private const string ApiRoute = "/api/projects";

    public ReadProjectTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    // Test of een user zonder rol een project kan lezen
    public async Task readProjects_ReturnsSuccess()
    {
        // Arrange: Zet een standaard user en testdata klaar
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(ApiRoute);

        // Assert: check voor success status code
        response.EnsureSuccessStatusCode();
        
    }
}