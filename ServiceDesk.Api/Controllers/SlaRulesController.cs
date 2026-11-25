using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Repositories;
using ServiceDesk.Api.DTOs;
using ServiceDesk.Api.Data; // Для AppDbContext, чтобы сохранять изменения
using Microsoft.EntityFrameworkCore;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SlaRulesController : ControllerBase
{
    private readonly ISlaRuleRepository _repo;
    private readonly AppDbContext _context;

    public SlaRulesController(ISlaRuleRepository repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var rules = await _repo.GetAllAsync();

        var result = rules.Select(r => new
        {
            r.Id,
            PriorityName = r.Priority?.Name ?? "Неизвестно",
            r.ResponseTimeMin,
            r.ResolutionTimeMin,
            r.IsActive
        });

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSlaRuleDto dto)
    {
        var rules = await _repo.GetAllAsync();
        var rule = rules.FirstOrDefault(r => r.Id == id);

        if (rule == null) return NotFound("Правило не найдено");

        rule.ResponseTimeMin = dto.ResponseTimeMin;
        rule.ResolutionTimeMin = dto.ResolutionTimeMin;
        rule.IsActive = dto.IsActive;

        await _repo.UpdateAsync(rule);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}