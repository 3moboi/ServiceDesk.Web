using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UsersController(IUserRepository repo)
    {
        _repo = repo;
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _repo.GetAllAsync();

        // Преобразуем сущности БД в анонимные объекты для отправки клиенту
        var result = users.Select(u => new
        {
            u.Id,
            FullName = $"{u.LastName} {u.FirstName}",
            u.Email,
            RoleName = u.Role?.Name ?? "Без роли",
            u.IsActive
        });

        return Ok(result);
    }

    // GET: api/users/executors
    [HttpGet("executors")]
    public async Task<IActionResult> GetExecutors()
    {
        var users = await _repo.GetAllAsync();
        var executors = users
            .Where(u => u.RoleId == 2) // Только исполнители
            .Select(u => new
            {
                u.Id,
                FullName = $"{u.LastName} {u.FirstName}"
            });

        return Ok(executors);
    }
}
