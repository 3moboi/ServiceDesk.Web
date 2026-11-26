using Moq;
using ServiceDesk.Api.Models;
using ServiceDesk.Api.Repositories;
using ServiceDesk.Api.Services;
using Xunit;

namespace ServiceDesk.Tests;

public class SlaServiceTests
{
    [Fact] // TC-SLA-01: Позитивный (Активное правило существует)
    public async Task CalculateDueDate_ActiveRuleExists_ReturnsCreatedAtPlusResolutionTime()
    {
        // Arrange
        var createdAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var rule = new SlaRule { Id = 1, PriorityId = 3, ResolutionTimeMin = 480, IsActive = true };

        var slaRepoMock = new Mock<ISlaRuleRepository>();
        slaRepoMock.Setup(r => r.GetActiveByPriorityAsync(3)).ReturnsAsync(rule);

        var service = new SlaService(slaRepoMock.Object);

        // Act
        var dueDate = await service.CalculateDueDateAsync(3, createdAt);

        // Assert
        Assert.Equal(createdAt.AddMinutes(480), dueDate);
    }

    [Fact] // TC-SLA-02: Негативный (Правила нет)
    public async Task CalculateDueDate_NoActiveRule_ThrowsBusinessRuleException()
    {
        // Arrange
        var slaRepoMock = new Mock<ISlaRuleRepository>();
        slaRepoMock.Setup(r => r.GetActiveByPriorityAsync(It.IsAny<int>())).ReturnsAsync((SlaRule?)null);

        var service = new SlaService(slaRepoMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.CalculateDueDateAsync(3, DateTime.UtcNow));
    }

    [Fact] // TC-SLA-03: Граничный (Время решения 1 минута)
    public async Task CalculateDueDate_OneMinuteRule_ReturnsCreatedAtPlusOneMinute()
    {
        // Arrange
        var createdAt = DateTime.UtcNow;
        var rule = new SlaRule { Id = 1, PriorityId = 4, ResolutionTimeMin = 1, IsActive = true };

        var slaRepoMock = new Mock<ISlaRuleRepository>();
        slaRepoMock.Setup(r => r.GetActiveByPriorityAsync(4)).ReturnsAsync(rule);

        var service = new SlaService(slaRepoMock.Object);

        // Act
        var dueDate = await service.CalculateDueDateAsync(4, createdAt);

        // Assert
        Assert.Equal(createdAt.AddMinutes(1), dueDate);
    }
}