namespace ServiceDesk.Api.DTOs;

// DTO для создания заявки
public class CreateTicketDto
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int PriorityId { get; set; }
}

// DTO для возврата ID созданной заявки
public class CreatedTicketDto
{
    public int Id { get; set; }
}

// DTO для смены статуса
public class UpdateStatusDto
{
    public int NewStatusId { get; set; }
    public string? Comment { get; set; }
    public int? AssigneeId { get; set; }
}

// DTO для добавления комментария
public class CreateCommentDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
}

// DTO для списка заявок
public class TicketListItemDto
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string PriorityName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string? AssigneeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
}

// DTO для карточки заявки
public class TicketDetailsDto
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string PriorityName { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string? AssigneeName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
    public List<StatusHistoryDto> History { get; set; } = new();
}

// DTO для комментария
public class CommentDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsInternal { get; set; }
}

// DTO для истории статусов
public class StatusHistoryDto
{
    public string? OldStatusName { get; set; }
    public string NewStatusName { get; set; } = string.Empty;
    public string ChangedByName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}

// DTO для отчётов
public class ReportDto
{
    public List<CategoryReportItem> ByCategory { get; set; } = new();
    public List<AssigneeReportItem> ByAssignee { get; set; } = new();
    public double OnTimePercentage { get; set; }
    public int TotalClosed { get; set; }
}

public class CategoryReportItem
{
    public string CategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AssigneeReportItem
{
    public string AssigneeName { get; set; } = string.Empty;
    public int Count { get; set; }
}