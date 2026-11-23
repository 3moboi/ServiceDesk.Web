using Microsoft.AspNetCore.Mvc;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    // GET: api/categories
    [HttpGet]
    public IActionResult Get()
    {
        // Возвращаем тестовые категории, чтобы выпадающий список в UI не был пустым
        var categories = new List<object>
        {
            new { Id = 1, Name = "Оборудование" },
            new { Id = 2, Name = "Программное обеспечение" },
            new { Id = 3, Name = "Сеть" },
            new { Id = 4, Name = "Учётные записи" }
        };
        return Ok(categories);
    }
}