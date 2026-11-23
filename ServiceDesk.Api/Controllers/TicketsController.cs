using Microsoft.AspNetCore.Mvc;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new List<object>());

    //Чтобы посмотреть заявку в браузере открыть https://localhost:7197/Tickets/Details/1
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        // Временная заглушка с тестовыми данными для проверки интерфейса
        var ticket = new
        {
            Id = id,
            Subject = "Не включается монитор",
            Description = "При нажатии кнопки питания монитор не реагирует, индикатор не горит.",
            CategoryName = "Оборудование",
            PriorityName = "Высокий",
            StatusName = "В работе",
            StatusId = 2,
            RequesterName = "Иванов Иван",
            AssigneeName = "Петров Петр",
            CreatedAt = DateTime.Now.AddHours(-2),
            AssignedAt = DateTime.Now.AddHours(-1),
            DueDate = DateTime.Now.AddHours(4),
            ClosedAt = (DateTime?)null,
            Comments = new List<object>
        {
            new { Id = 1, AuthorName = "Иванов Иван", Text = "Проблема появилась утром.", CreatedAt = DateTime.Now.AddHours(-2), IsInternal = false },
            new { Id = 2, AuthorName = "Петров Петр", Text = "Проверяю кабели питания.", CreatedAt = DateTime.Now.AddHours(-1), IsInternal = true },
            new { Id = 3, AuthorName = "Петров Петр", Text = "Вышел из строя блок питания, меняю.", CreatedAt = DateTime.Now.AddMinutes(-30), IsInternal = false }
        },
            History = new List<object>
        {
            new { OldStatusName = (string?)null, NewStatusName = "Новая", ChangedByName = "Иванов Иван", ChangedAt = DateTime.Now.AddHours(-2) },
            new { OldStatusName = "Новая", NewStatusName = "В работе", ChangedByName = "Петров Петр", ChangedAt = DateTime.Now.AddHours(-1) }
        }
        };

        return Ok(ticket);
    }

    [HttpGet("categories")]
    public IActionResult GetCategories() => Ok(new List<object>());
}