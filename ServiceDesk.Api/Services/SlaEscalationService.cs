using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Services;

public class SlaEscalationService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SlaEscalationService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

    public SlaEscalationService(IServiceScopeFactory scopeFactory, ILogger<SlaEscalationService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var ticketRepo = scope.ServiceProvider.GetRequiredService<ITicketRepository>();

            var overdue = await ticketRepo.GetOverdueAsync(DateTime.UtcNow);
            foreach (var ticket in overdue)
            {
                _logger.LogWarning("Заявка {TicketId} просрочена. Плановый срок: {DueDate}", ticket.Id, ticket.DueDate);
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}