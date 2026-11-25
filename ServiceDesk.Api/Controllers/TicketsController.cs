using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.DTOs;
using ServiceDesk.Api.Repositories;
using ServiceDesk.Api.Services;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ITicketRepository _ticketRepo;
    private readonly IUserRepository _userRepo;

    // Временно считаем, что текущий пользователь - ID 1 (заявитель)
    // Для реального проекта тут брался бы ID из токена авторизации
    private int CurrentUserId
    {
        get
        {
            if (Request.Headers.TryGetValue("X-User-Id", out var idStr) && int.TryParse(idStr, out var id))
                return id;
            return 1; // По умолчанию
        }
    }

    public TicketsController(ITicketService ticketService, ITicketRepository ticketRepo, IUserRepository userRepo)
    {
        _ticketService = ticketService;
        _ticketRepo = ticketRepo;
        _userRepo = userRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? statusId, [FromQuery] int? categoryId, [FromQuery] int? priorityId, [FromQuery] string? search)
    {
        var currentUser = await _userRepo.GetByIdAsync(CurrentUserId);
        if (currentUser == null) return Unauthorized();

        var tickets = await _ticketRepo.GetFilteredAsync(statusId, categoryId, priorityId, search);

        // Фильтрация по ролям
        if (currentUser.RoleId == 1) // Заявитель
        {
            tickets = tickets.Where(t => t.RequesterId == CurrentUserId).ToList();
        }
        else if (currentUser.RoleId == 2) // Исполнитель
        {
            tickets = tickets.Where(t => t.AssigneeId == CurrentUserId).ToList();
        }
        // Если RoleId == 3 (Администратор), он видит все заявки без фильтрации

        var result = tickets.Select(t => new TicketListItemDto
        {
            Id = t.Id,
            Subject = t.Subject,
            CategoryName = t.Category?.Name ?? "",
            PriorityName = t.Priority?.Name ?? "",
            StatusName = t.Status?.Name ?? "",
            AssigneeName = t.Assignee != null ? $"{t.Assignee.LastName} {t.Assignee.FirstName}" : null,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate
        });
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _ticketRepo.GetByIdAsync(id);
        if (t == null) return NotFound();

        var result = new TicketDetailsDto
        {
            Id = t.Id,
            Subject = t.Subject,
            Description = t.Description,
            CategoryName = t.Category?.Name ?? "",
            PriorityName = t.Priority?.Name ?? "",
            StatusName = t.Status?.Name ?? "",
            StatusId = t.StatusId,
            RequesterName = $"{t.Requester?.LastName} {t.Requester?.FirstName}",
            AssigneeName = t.Assignee != null ? $"{t.Assignee.LastName} {t.Assignee.FirstName}" : null,
            CreatedAt = t.CreatedAt,
            AssignedAt = t.AssignedAt,
            DueDate = t.DueDate,
            ClosedAt = t.ClosedAt,
            Comments = t.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                AuthorName = $"{c.Author?.LastName} {c.Author?.FirstName}",
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                IsInternal = c.IsInternal
            }).ToList(),
            History = t.History.Select(h => new StatusHistoryDto
            {
                OldStatusName = h.OldStatus?.Name,
                NewStatusName = h.NewStatus?.Name ?? "",
                ChangedByName = $"{h.ChangedByUser?.LastName} {h.ChangedByUser?.FirstName}",
                ChangedAt = h.ChangedAt
            }).ToList()
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var id = await _ticketService.CreateAsync(dto, CurrentUserId);
        return Ok(new { Id = id });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        await _ticketService.ChangeStatusAsync(id, dto.NewStatusId, dto.Comment, dto.AssigneeId, CurrentUserId);
        return NoContent();
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentDto dto)
    {
        await _ticketService.AddCommentAsync(id, dto, CurrentUserId);
        return NoContent();
    }
}