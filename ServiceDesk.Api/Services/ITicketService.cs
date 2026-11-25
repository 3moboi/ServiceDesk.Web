using ServiceDesk.Api.DTOs;

namespace ServiceDesk.Api.Services;

public interface ITicketService
{
    Task<int> CreateAsync(CreateTicketDto dto, int requesterId);
    Task AssignAsync(int ticketId, int assigneeId, int changedByUserId);
    Task ChangeStatusAsync(int ticketId, int newStatusId, string? comment, int? assigneeId, int changedByUserId);
    Task AddCommentAsync(int ticketId, CreateCommentDto dto, int authorId);
}

public interface ISlaService
{
    Task<DateTime> CalculateDueDateAsync(int priorityId, DateTime createdAt);
}