using Microsoft.AspNetCore.Mvc;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrioritiesController : ControllerBase
{
    // GET: api/priorities
    [HttpGet]
    public IActionResult Get()
    {
        // Возвращаем тестовые приоритеты для выпадающего списка
        var priorities = new List<object>
        {
            new { Id = 1, Name = "Низкий" },
            new { Id = 2, Name = "Средний" },
            new { Id = 3, Name = "Высокий" },
            new { Id = 4, Name = "Критический" }
        };
        return Ok(priorities);
    }
}