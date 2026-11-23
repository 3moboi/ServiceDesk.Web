namespace ServiceDesk.Web.Models.Api;

public class TicketFilterDto
{
    public int? StatusId { get; set; }
    public int? CategoryId { get; set; }
    public int? PriorityId { get; set; }
    public string? Search { get; set; }
}

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

public class CommentDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsInternal { get; set; }
}

public class StatusHistoryDto
{
    public string? OldStatusName { get; set; }
    public string NewStatusName { get; set; } = string.Empty;
    public string ChangedByName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}

public class CreateTicketDto
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int PriorityId { get; set; }
}

public class CreatedTicketDto
{
    public int Id { get; set; }
}

public class UpdateStatusDto
{
    public int NewStatusId { get; set; }
    public string? Comment { get; set; }
}

public class CreateCommentDto
{
    public string Text { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
}