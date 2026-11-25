using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrioritiesController : ControllerBase
{

    private readonly IPriorityRepository _repo;

    public PrioritiesController(IPriorityRepository repo)
    {
        _repo = repo;
    }

    // GET: api/priorities
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var priorities = await _repo.GetAllAsync();

        // Возвращаем только Id и Name, чтобы не светить лишние поля (например, Weight)
        var result = priorities.Select(p => new
        {
            p.Id,
            p.Name
        });

        return Ok(result);
    }
}