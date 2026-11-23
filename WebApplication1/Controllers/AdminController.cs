using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Web.Models.Api;
using ServiceDesk.Web.Services;
using ServiceDesk.Web.ViewModels;

namespace ServiceDesk.Web.Controllers;

public class AdminController : Controller
{
    private readonly IServiceDeskApiClient _api;

    public AdminController(IServiceDeskApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> SlaRules(CancellationToken ct)
    {
        var vm = new SlaRulesViewModel
        {
            Rules = await _api.GetSlaRulesAsync(ct)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSlaRule(int id, UpdateSlaRuleDto dto, CancellationToken ct)
    {
        try
        {
            await _api.UpdateSlaRuleAsync(id, dto, ct);
            TempData["Success"] = "Правило SLA обновлено";
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(SlaRules));
    }

    [HttpGet]
    public async Task<IActionResult> Users(CancellationToken ct)
    {
        var vm = new UsersViewModel
        {
            Users = await _api.GetUsersAsync(ct)
        };
        return View(vm);
    }
}