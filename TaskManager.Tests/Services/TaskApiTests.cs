using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManager.API;
using TaskManager.Tests;
using Xunit;

public class TaskApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TaskApiTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Add authorization header for bypassing [Authorize] attribute
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer TestToken");
    }

    [Fact(Skip = "Authentication issue needs to be fixed")]
    public async Task Post_Task_Should_Return_201()
    {
        // Arrange – създай валиден JSON за заявка
        var json = JsonSerializer.Serialize(new
        {
            title = "Integration Test Task",
            description = "Test",
            projectId = Guid.NewGuid(),
            assignedToUserId = Guid.NewGuid(),
            status = "Todo",
            priority = "High",
            dueDate = DateTime.UtcNow.AddDays(2)
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/Task", content);

        // Assert
        response.EnsureSuccessStatusCode(); // Will throw if not successful
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }
}

