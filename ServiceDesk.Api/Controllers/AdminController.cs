using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Api.Repositories;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly ISlaRuleRepository _slaRepo;

    public AdminController(IUserRepository userRepo, ISlaRuleRepository slaRepo)
    {
        _userRepo = userRepo;
        _slaRepo = slaRepo;
    }

    // Вспомогательный метод для получения ID текущего пользователя из заголовка
    private int CurrentUserId
    {
        get
        {
            if (Request.Headers.TryGetValue("X-User-Id", out var idStr) && int.TryParse(idStr, out var id))
                return id;
            return 1; // По умолчанию
        }
    }

    // GET: api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        // В реальном проекте здесь была бы проверка: если CurrentUserId не админ, вернуть 403 Forbidden
        var users = await _userRepo.GetAllAsync();
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

    // GET: api/admin/sla-rules
    [HttpGet("sla-rules")]
    public async Task<IActionResult> GetSlaRules()
    {
        var rules = await _slaRepo.GetAllAsync();
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
}