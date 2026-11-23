using Microsoft.AspNetCore.Mvc;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    // GET: api/reports?from=2023-01-01&to=2023-12-31
    [HttpGet]
    public IActionResult Get([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        // Возвращаем пустой отчёт, чтобы UI отрисовал структуру
        var emptyReport = new
        {
            ByCategory = new List<object>(),
            ByAssignee = new List<object>(),
            OnTimePercentage = 0.0,
            TotalClosed = 0
        };
        return Ok(emptyReport);
    }
}