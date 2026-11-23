using Microsoft.AspNetCore.Mvc;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SlaRulesController : ControllerBase
{
    // GET: api/slarules
    [HttpGet]
    public IActionResult Get()
    {
        var rules = new List<object>
        {
            new { Id = 1, PriorityName = "Низкий", ResponseTimeMin = 480, ResolutionTimeMin = 2880, IsActive = true },
            new { Id = 2, PriorityName = "Средний", ResponseTimeMin = 240, ResolutionTimeMin = 1440, IsActive = true },
            new { Id = 3, PriorityName = "Высокий", ResponseTimeMin = 60, ResolutionTimeMin = 480, IsActive = true },
            new { Id = 4, PriorityName = "Критический", ResponseTimeMin = 15, ResolutionTimeMin = 120, IsActive = true }
        };
        return Ok(rules);
    }

    // PUT: api/slarules/5
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] object dto)
    {
        return NoContent();
    }
}