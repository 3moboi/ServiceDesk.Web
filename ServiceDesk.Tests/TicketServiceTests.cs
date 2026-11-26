using Microsoft.EntityFrameworkCore;
using Moq;
using ServiceDesk.Api.Data;
using ServiceDesk.Api.DTOs;
using ServiceDesk.Api.Models;
using ServiceDesk.Api.Repositories;
using ServiceDesk.Api.Services;
using Xunit;

namespace ServiceDesk.Tests;

public class TicketServiceTests
{
    // Вспомогательный метод для создания контекста БД в памяти
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    // ==========================================
    // ТЕСТЫ РЕГИСТРАЦИИ ЗАЯВКИ (CreateAsync)
    // ==========================================

    [Fact] // TC-CRT-01: Позитивный
    public async Task CreateAsync_ValidData_CreatesTicketAndHistory()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var user = new User { Id = 1, RoleId = 1 };
        var category = new Category { Id = 1, Name = "Оборудование" };
        var priority = new Priority { Id = 1, Name = "Низкий" };

        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        var categoryRepoMock = new Mock<ICategoryRepository>();
        categoryRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);
        var priorityRepoMock = new Mock<IPriorityRepository>();
        priorityRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(priority);
        var slaServiceMock = new Mock<ISlaService>();
        slaServiceMock.Setup(s => s.CalculateDueDateAsync(1, It.IsAny<DateTime>())).ReturnsAsync(DateTime.UtcNow.AddHours(8));

        var ticketRepoMock = new Mock<ITicketRepository>();
        var service = new TicketService(ticketRepoMock.Object, userRepoMock.Object,
            categoryRepoMock.Object, priorityRepoMock.Object, slaServiceMock.Object, context);

        var dto = new CreateTicketDto { Subject = "Тест", Description = "Описание", CategoryId = 1, PriorityId = 1 };

        // Act
        var ticketId = await service.CreateAsync(dto, 1);

        // Assert
        Assert.True(ticketId > 0);
        var ticket = await context.Tickets.FindAsync(ticketId);
        Assert.NotNull(ticket);
        Assert.Equal(1, ticket.StatusId); // "Новая"
        Assert.Equal(1, ticket.RequesterId);
    }

    [Fact] // TC-CRT-02: Негативный (Нет пользователя)
    public async Task CreateAsync_UserNotFound_ThrowsBusinessRuleException()
    {
        var context = GetInMemoryDbContext();
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User?)null);

        var service = new TicketService(null!, userRepoMock.Object, null!, null!, null!, context);
        var dto = new CreateTicketDto();

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.CreateAsync(dto, 1));
    }

    // ==========================================
    // ТЕСТЫ НАЗНАЧЕНИЯ ИСПОЛНИТЕЛЯ (AssignAsync)
    // ==========================================

    [Fact] // TC-ASG-02: Негативный (Пользователь не исполнитель)
    public async Task AssignAsync_UserIsNotExecutor_ThrowsBusinessRuleException()
    {
        var ticket = new Ticket { Id = 1, StatusId = 1 }; // Новая
        var user = new User { Id = 1, RoleId = 1 }; // Заявитель!

        var ticketRepoMock = new Mock<ITicketRepository>();
        ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

        var service = new TicketService(ticketRepoMock.Object, userRepoMock.Object,
            null!, null!, null!, GetInMemoryDbContext());

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.AssignAsync(1, 1, 1));
    }

    [Fact] // TC-ASG-03: Негативный (Заявка закрыта)
    public async Task AssignAsync_TicketIsClosed_ThrowsBusinessRuleException()
    {
        var ticket = new Ticket { Id = 1, StatusId = 4 }; // Закрыта
        var user = new User { Id = 2, RoleId = 2 }; // Исполнитель

        var ticketRepoMock = new Mock<ITicketRepository>();
        ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(user);

        var service = new TicketService(ticketRepoMock.Object, userRepoMock.Object,
            null!, null!, null!, GetInMemoryDbContext());

        await Assert.ThrowsAsync<BusinessRuleException>(() => service.AssignAsync(1, 2, 2));
    }

    // ==========================================
    // ТЕСТЫ СМЕНЫ СТАТУСА (ChangeStatusAsync)
    // ==========================================

    [Fact] // TC-STS-01: Позитивный (Новая -> В работе)
    public async Task ChangeStatus_NewToInProgress_UpdatesStatus()
    {
        var context = GetInMemoryDbContext();
        var ticket = new Ticket { Id = 1, StatusId = 1 }; // Новая
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();

        var ticketRepoMock = new Mock<ITicketRepository>();
        ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);

        var service = new TicketService(ticketRepoMock.Object, null!, null!, null!, null!, context);

        await service.ChangeStatusAsync(1, newStatusId: 2, comment: null, assigneeId: null, changedByUserId: 2);

        Assert.Equal(2, ticket.StatusId);
    }

    [Fact] // TC-STS-03: Негативный (Новая -> Закрыта)
    public async Task ChangeStatus_FromNewToClosed_ThrowsBusinessRuleException()
    {
        var ticket = new Ticket { Id = 1, StatusId = 1 }; // Новая
        var ticketRepoMock = new Mock<ITicketRepository>();
        ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ticket);

        var service = new TicketService(ticketRepoMock.Object, null!, null!, null!, null!, GetInMemoryDbContext());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => service.ChangeStatusAsync(1, newStatusId: 4, comment: null, assigneeId: null, changedByUserId: 2));

        Assert.Contains("Переход в 'Закрыта' возможен только из 'Решена'", exception.Message);
    }

    [Fact] // TC-STS-04: Негативный (Заявки нет)
    public async Task ChangeStatus_TicketNotFound_ThrowsNotFoundException()
    {
        var ticketRepoMock = new Mock<ITicketRepository>();
        ticketRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket?)null);

        var service = new TicketService(ticketRepoMock.Object, null!, null!, null!, null!, GetInMemoryDbContext());

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.ChangeStatusAsync(99, 2, null, null, 1));
    }
}