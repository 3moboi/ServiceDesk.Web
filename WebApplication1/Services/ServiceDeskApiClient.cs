using ServiceDesk.Web.Models.Api;
using System.Net;
using System.Net.Http.Json;
using ServiceDesk.Web.Services;

namespace ServiceDesk.Web.Services;

public class ServiceDeskApiClient : IServiceDeskApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServiceDeskApiClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
    }

    private void SetUserHeader()
    {
        var userId = _httpContextAccessor.HttpContext?.Session.GetInt32("UserId");
        _http.DefaultRequestHeaders.Remove("X-User-Id"); // Удаляем старый заголовок
        if (userId.HasValue)
        {
            _http.DefaultRequestHeaders.Add("X-User-Id", userId.Value.ToString());
        }
    }

    public async Task<IReadOnlyList<TicketListItemDto>> GetTicketsAsync(
        TicketFilterDto filter, CancellationToken ct = default)
    {
        SetUserHeader();
        var query = BuildQuery(filter);
        var result = await GetAsync<List<TicketListItemDto>>($"api/tickets{query}", ct);
        return result ?? new List<TicketListItemDto>();
    }

    public async Task<IReadOnlyList<UserDto>> GetExecutorsAsync(CancellationToken ct = default)
    {
        SetUserHeader();
        return await GetAsync<List<UserDto>>("api/users/executors", ct) ?? new();
    }

    public async Task<TicketDetailsDto?> GetTicketByIdAsync(int id, CancellationToken ct = default)
    {
        SetUserHeader();
        var response = await _http.GetAsync($"api/tickets/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<TicketDetailsDto>(cancellationToken: ct);
    }

    public async Task<int> CreateTicketAsync(CreateTicketDto dto, CancellationToken ct = default)
    {
        SetUserHeader();
        var response = await _http.PostAsJsonAsync("api/tickets", dto, ct);
        await EnsureSuccessAsync(response);

        var created = await response.Content.ReadFromJsonAsync<CreatedTicketDto>(cancellationToken: ct);
        if (created is null)
            throw new ApiException((int)response.StatusCode, "Сервис не вернул идентификатор заявки");

        return created.Id;
    }

    public async Task UpdateTicketStatusAsync(int id, UpdateStatusDto dto, CancellationToken ct = default)
    {
        SetUserHeader();
        var response = await _http.PutAsJsonAsync($"api/tickets/{id}/status", dto, ct);
        await EnsureSuccessAsync(response);
    }

    public async Task AddCommentAsync(int id, CreateCommentDto dto, CancellationToken ct = default)
    {
        SetUserHeader();
        var response = await _http.PostAsJsonAsync($"api/tickets/{id}/comments", dto, ct);
        await EnsureSuccessAsync(response);
    }

    public async Task<ReportDto> GetReportAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        SetUserHeader();
        var result = await GetAsync<ReportDto>(
            $"api/reports?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", ct);
        return result ?? new ReportDto();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default)
    {
        SetUserHeader();
        return await GetAsync<List<CategoryDto>>("api/categories", ct) ?? new();
    }

    public async Task<IReadOnlyList<PriorityDto>> GetPrioritiesAsync(CancellationToken ct = default)
    { 
        SetUserHeader();
        return await GetAsync<List<PriorityDto>>("api/priorities", ct) ?? new(); 
    }

    public async Task<IReadOnlyList<SlaRuleDto>> GetSlaRulesAsync(CancellationToken ct = default)
    { 
        SetUserHeader();
        return await GetAsync<List<SlaRuleDto>>("api/slarules", ct) ?? new(); 
    }

    public async Task UpdateSlaRuleAsync(int id, UpdateSlaRuleDto dto, CancellationToken ct = default)
    {
        SetUserHeader();
        var response = await _http.PutAsJsonAsync($"api/slarules/{id}", dto, ct);
        await EnsureSuccessAsync(response);
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken ct = default)
    {
        SetUserHeader();
        return await GetAsync<List<UserDto>>("api/users", ct) ?? new();
    }

    private async Task<T?> GetAsync<T>(string url, CancellationToken ct)
    {
        var response = await _http.GetAsync(url, ct);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync();
        throw new ApiException((int)response.StatusCode,
            string.IsNullOrWhiteSpace(body) ? response.ReasonPhrase ?? "Ошибка веб-сервиса" : body);
    }

    private static string BuildQuery(TicketFilterDto filter)
    {
        var parts = new List<string>();
        if (filter.StatusId.HasValue) parts.Add($"statusId={filter.StatusId}");
        if (filter.CategoryId.HasValue) parts.Add($"categoryId={filter.CategoryId}");
        if (filter.PriorityId.HasValue) parts.Add($"priorityId={filter.PriorityId}");
        if (!string.IsNullOrWhiteSpace(filter.Search))
            parts.Add($"search={Uri.EscapeDataString(filter.Search)}");

        return parts.Count == 0 ? string.Empty : "?" + string.Join("&", parts);
    }
}