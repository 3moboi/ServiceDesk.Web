using ServiceDesk.Web.Models.Api;

namespace ServiceDesk.Web.ViewModels;

public class TicketListViewModel
{
    public IReadOnlyList<TicketListItemDto> Tickets { get; set; } = Array.Empty<TicketListItemDto>();
    public TicketFilterDto Filter { get; set; } = new();
    public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
    public IReadOnlyList<PriorityDto> Priorities { get; set; } = Array.Empty<PriorityDto>();
}