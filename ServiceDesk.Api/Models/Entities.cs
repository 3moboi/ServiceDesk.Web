using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceDesk.Api.Models;

public class Role
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}

public class User
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Category
{
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

public class Priority
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public int Weight { get; set; }
}

public class Status
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}

public class SlaRule
{
    public int Id { get; set; }
    public int PriorityId { get; set; }
    public Priority Priority { get; set; } = null!;

    public int ResponseTimeMin { get; set; }
    public int ResolutionTimeMin { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Ticket
{
    public int Id { get; set; }
    [Required, MaxLength(200)]
    public string Subject { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;

    public int RequesterId { get; set; }
    public User Requester { get; set; } = null!;

    public int? AssigneeId { get; set; }
    public User? Assignee { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public int PriorityId { get; set; }
    public Priority Priority { get; set; } = null!;

    public int StatusId { get; set; }
    public Status Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AssignedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ClosedAt { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TicketStatusHistory> History { get; set; } = new List<TicketStatusHistory>();
}

public class Comment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    [Required]
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsInternal { get; set; }
}

public class TicketStatusHistory
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;

    public int? OldStatusId { get; set; }
    public Status? OldStatus { get; set; }

    public int NewStatusId { get; set; }
    public Status NewStatus { get; set; } = null!;

    public int ChangedByUserId { get; set; }
    public User ChangedByUser { get; set; } = null!;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}