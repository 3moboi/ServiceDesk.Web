using ServiceDesk.Web.Models.Api;

namespace ServiceDesk.Web.ViewModels;

public class TicketDetailsViewModel
{
    public TicketDetailsDto Ticket { get; set; } = new();
    public bool CanChangeStatus { get; set; }

    public IReadOnlyList<UserDto> Executors { get; set; } = Array.Empty<UserDto>();
    public bool CanSeeInternalComments { get; set; }
    public string CurrentUserRole { get; set; } = string.Empty;
}