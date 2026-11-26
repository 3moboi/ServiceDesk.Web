using ServiceDesk.Api.Data;
using ServiceDesk.Api.DTOs;
using ServiceDesk.Api.Models;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepo;
    private readonly IUserRepository _userRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly IPriorityRepository _priorityRepo;
    private readonly ISlaService _slaService;
    private readonly AppDbContext _context;

    public TicketService(ITicketRepository ticketRepo, IUserRepository userRepo,
        ICategoryRepository categoryRepo, IPriorityRepository priorityRepo,
        ISlaService slaService, AppDbContext context)
    {
        _ticketRepo = ticketRepo;
        _userRepo = userRepo;
        _categoryRepo = categoryRepo;
        _priorityRepo = priorityRepo;
        _slaService = slaService;
        _context = context;
    }

    public async Task<int> CreateAsync(CreateTicketDto dto, int requesterId)
    {
        var requester = await _userRepo.GetByIdAsync(requesterId)
            ?? throw new BusinessRuleException("Заявитель не найден");
        var category = await _categoryRepo.GetByIdAsync(dto.CategoryId)
            ?? throw new BusinessRuleException("Категория не найдена");
        var priority = await _priorityRepo.GetByIdAsync(dto.PriorityId)
            ?? throw new BusinessRuleException("Приоритет не найден");

        var now = DateTime.UtcNow;
        var dueDate = await _slaService.CalculateDueDateAsync(priority.Id, now);

        var ticket = new Ticket
        {
            Subject = dto.Subject,
            Description = dto.Description,
            RequesterId = requester.Id,
            CategoryId = category.Id,
            PriorityId = priority.Id,
            StatusId = 1, // 1 = "Новая"
            CreatedAt = now,
            DueDate = dueDate
        };

        //await _ticketRepo.AddAsync(ticket);
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();

        await _ticketRepo.AddStatusHistoryAsync(new TicketStatusHistory
        {
            Ticket = ticket,
            OldStatusId = null,
            NewStatusId = 1,
            ChangedByUserId = requester.Id,
            ChangedAt = now
        });

        await _context.SaveChangesAsync();
        return ticket.Id;
    }

    public async Task AssignAsync(int ticketId, int assigneeId, int changedByUserId)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId)
            ?? throw new NotFoundException("Заявка не найдена");
        var assignee = await _userRepo.GetByIdAsync(assigneeId)
            ?? throw new BusinessRuleException("Исполнитель не найден");

        if (assignee.RoleId != 2) // 2 = "Исполнитель"
            throw new BusinessRuleException("Пользователь не имеет роли исполнителя");

        if (ticket.StatusId != 1 && ticket.StatusId != 2)
            throw new BusinessRuleException("Нельзя назначить исполнителя на эту заявку");

        var now = DateTime.UtcNow;
        ticket.AssigneeId = assignee.Id;
        ticket.AssignedAt = now;

        if (ticket.StatusId == 1)
        {
            int oldStatus = ticket.StatusId;
            ticket.StatusId = 2; // "В работе"

            await _ticketRepo.AddStatusHistoryAsync(new TicketStatusHistory
            {
                TicketId = ticket.Id,
                OldStatusId = oldStatus,
                NewStatusId = 2,
                ChangedByUserId = changedByUserId,
                ChangedAt = now
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task ChangeStatusAsync(int ticketId, int newStatusId, string? comment, int? assigneeId, int changedByUserId)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId)
            ?? throw new NotFoundException("Заявка не найдена");

        // Простая проверка переходов: 1->2->3->4
        if (newStatusId == 2 && ticket.StatusId != 1)
            throw new BusinessRuleException("Переход в 'В работе' возможен только из 'Новая'");
        if (newStatusId == 3 && ticket.StatusId != 2)
            throw new BusinessRuleException("Переход в 'Решена' возможен только из 'В работе'");
        if (newStatusId == 4 && ticket.StatusId != 3)
            throw new BusinessRuleException("Переход в 'Закрыта' возможен только из 'Решена'");

        var now = DateTime.UtcNow;

        // Если назначен исполнитель — обновляем его
        if (assigneeId.HasValue)
        {
            ticket.AssigneeId = assigneeId.Value;
            ticket.AssignedAt = now;
        }

        int oldStatus = ticket.StatusId;
        ticket.StatusId = newStatusId;

        if (newStatusId == 4) ticket.ClosedAt = DateTime.UtcNow;

        await _ticketRepo.AddStatusHistoryAsync(new TicketStatusHistory
        {
            TicketId = ticket.Id,
            OldStatusId = oldStatus,
            NewStatusId = newStatusId,
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTime.UtcNow,
            // Comment = comment // Если добавите это поле в модель, раскомментируйте
        });

        await _context.SaveChangesAsync();
    }

    public async Task AddCommentAsync(int ticketId, CreateCommentDto dto, int authorId)
    {
        var ticket = await _ticketRepo.GetByIdAsync(ticketId)
            ?? throw new NotFoundException("Заявка не найдена");

        var comment = new Comment
        {
            TicketId = ticket.Id,
            AuthorId = authorId,
            Text = dto.Text,
            CreatedAt = DateTime.UtcNow,
            IsInternal = dto.IsInternal
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }
}