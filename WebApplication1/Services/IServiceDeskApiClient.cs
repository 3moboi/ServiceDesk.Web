using ServiceDesk.Web.Models.Api;

namespace ServiceDesk.Web.Services
{
    public interface IServiceDeskApiClient
    {
        Task<IReadOnlyList<TicketListItemDto>> GetTicketsAsync(
        TicketFilterDto filter, CancellationToken ct = default);

    Task<TicketDetailsDto?> GetTicketByIdAsync(int id, CancellationToken ct = default);

    Task<int> CreateTicketAsync(CreateTicketDto dto, CancellationToken ct = default);

    Task UpdateTicketStatusAsync(int id, UpdateStatusDto dto, CancellationToken ct = default);

    Task AddCommentAsync(int id, CreateCommentDto dto, CancellationToken ct = default);

    Task<ReportDto> GetReportAsync(DateTime from, DateTime to, CancellationToken ct = default);

    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<PriorityDto>> GetPrioritiesAsync(CancellationToken ct = default);

    Task<IReadOnlyList<SlaRuleDto>> GetSlaRulesAsync(CancellationToken ct = default);

    Task UpdateSlaRuleAsync(int id, UpdateSlaRuleDto dto, CancellationToken ct = default);

    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default);
    }
}

