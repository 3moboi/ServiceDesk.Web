using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    public CategoriesController(ICategoryRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var categories = await _repo.GetAllAsync();
        return Ok(categories.Select(c => new { c.Id, c.Name }));
    }
}