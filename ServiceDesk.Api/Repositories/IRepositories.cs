using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Repositories;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(int id);
    Task<List<Ticket>> GetFilteredAsync(int? statusId, int? categoryId, int? priorityId, string? search);
    Task AddAsync(Ticket ticket);
    Task AddStatusHistoryAsync(TicketStatusHistory history);
    Task<List<Ticket>> GetOverdueAsync(DateTime currentDate);
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> GetAllAsync();
}

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<List<Category>> GetAllAsync();
}

public interface IPriorityRepository
{
    Task<Priority?> GetByIdAsync(int id);
    Task<List<Priority>> GetAllAsync();
}

public interface ISlaRuleRepository
{
    Task<SlaRule?> GetActiveByPriorityAsync(int priorityId);
    Task<List<SlaRule>> GetAllAsync();
    Task UpdateAsync(SlaRule rule);
}