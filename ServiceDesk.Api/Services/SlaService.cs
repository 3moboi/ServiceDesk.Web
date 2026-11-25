using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Services;

public class SlaService : ISlaService
{
    private readonly ISlaRuleRepository _slaRepo;
    public SlaService(ISlaRuleRepository slaRepo) => _slaRepo = slaRepo;

    public async Task<DateTime> CalculateDueDateAsync(int priorityId, DateTime createdAt)
    {
        var rule = await _slaRepo.GetActiveByPriorityAsync(priorityId);
        if (rule is null)
            throw new BusinessRuleException($"Для приоритета {priorityId} не задано активное правило SLA");

        return createdAt.AddMinutes(rule.ResolutionTimeMin);
    }
}