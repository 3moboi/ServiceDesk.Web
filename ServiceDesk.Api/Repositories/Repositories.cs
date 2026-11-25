using Microsoft.EntityFrameworkCore;
using ServiceDesk.Api.Data;
using ServiceDesk.Api.Models;

namespace ServiceDesk.Api.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;
    public TicketRepository(AppDbContext context) => _context = context;

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Requester)
            .Include(t => t.Assignee)
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Comments).ThenInclude(c => c.Author)
            .Include(t => t.History).ThenInclude(h => h.OldStatus)
            .Include(t => t.History).ThenInclude(h => h.NewStatus)
            .Include(t => t.History).ThenInclude(h => h.ChangedByUser)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Ticket>> GetFilteredAsync(int? statusId, int? categoryId, int? priorityId, string? search)
    {
        var query = _context.Tickets
            .Include(t => t.Category)
            .Include(t => t.Priority)
            .Include(t => t.Status)
            .Include(t => t.Assignee)
            .AsQueryable();

        if (statusId.HasValue) query = query.Where(t => t.StatusId == statusId);
        if (categoryId.HasValue) query = query.Where(t => t.CategoryId == categoryId);
        if (priorityId.HasValue) query = query.Where(t => t.PriorityId == priorityId);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Subject.Contains(search));

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task AddAsync(Ticket ticket) => await _context.Tickets.AddAsync(ticket);

    public async Task AddStatusHistoryAsync(TicketStatusHistory history) => await _context.TicketStatusHistories.AddAsync(history);

    public async Task<List<Ticket>> GetOverdueAsync(DateTime currentDate)
    {
        return await _context.Tickets
            .Where(t => t.DueDate < currentDate && t.StatusId != 4 && t.StatusId != 3) // не Закрыта и не Решена
            .ToListAsync();
    }
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;
    public async Task<User?> GetByIdAsync(int id)
    {
        // Вот здесь уже пишем Include
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<List<User>> GetAllAsync() => await _context.Users.Include(u => u.Role).ToListAsync();
}

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    public CategoryRepository(AppDbContext context) => _context = context;
    public async Task<Category?> GetByIdAsync(int id) => await _context.Categories.FindAsync(id);
    public async Task<List<Category>> GetAllAsync() => await _context.Categories.ToListAsync();
}

public class PriorityRepository : IPriorityRepository
{
    private readonly AppDbContext _context;
    public PriorityRepository(AppDbContext context) => _context = context;
    public async Task<Priority?> GetByIdAsync(int id) => await _context.Priorities.FindAsync(id);
    public async Task<List<Priority>> GetAllAsync() => await _context.Priorities.ToListAsync();
}

public class SlaRuleRepository : ISlaRuleRepository
{
    private readonly AppDbContext _context;
    public SlaRuleRepository(AppDbContext context) => _context = context;

    public async Task<SlaRule?> GetActiveByPriorityAsync(int priorityId)
    {
        return await _context.SlaRules.FirstOrDefaultAsync(r => r.PriorityId == priorityId && r.IsActive);
    }
    public async Task<List<SlaRule>> GetAllAsync() => await _context.SlaRules.Include(r => r.Priority).ToListAsync();
    public Task UpdateAsync(SlaRule rule)
    {
        _context.SlaRules.Update(rule);
        return Task.CompletedTask;
    }
}