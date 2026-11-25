using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _reportRepo;

    public ReportsController(IReportRepository reportRepo)
    {
        _reportRepo = reportRepo;
    }

    // GET: api/reports?from=2023-01-01&to=2023-12-31
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var fromDate = from ?? DateTime.Today.AddDays(-30);
        var toDate = to ?? DateTime.Today;

        // Если передан только день, захватываем весь день до 23:59:59
        var endOfDay = toDate.Date.AddDays(1).AddTicks(-1);

        var report = await _reportRepo.GenerateReportAsync(fromDate, endOfDay);
        return Ok(report);
    }
}