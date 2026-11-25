using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Web.Models.Api;
using ServiceDesk.Web.Services;
using ServiceDesk.Web.ViewModels;

namespace ServiceDesk.Web.Controllers;

public class TicketsController : Controller
{
    private readonly IServiceDeskApiClient _api;

    public TicketsController(IServiceDeskApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] TicketFilterDto filter, CancellationToken ct)
    {
        
        var vm = new TicketListViewModel
        {
            Filter = filter,
            Tickets = await _api.GetTicketsAsync(filter, ct),
            Categories = await _api.GetCategoriesAsync(ct),
            Priorities = await _api.GetPrioritiesAsync(ct)
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var ticket = await _api.GetTicketByIdAsync(id, ct);
        if (ticket is null)
            return NotFound();

        var vm = new TicketDetailsViewModel
        {
            Ticket = ticket,
            CanChangeStatus = true, // в реальном проекте — на основе роли
            CanSeeInternalComments = true,
            Executors = await _api.GetExecutorsAsync(ct)
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new CreateTicketViewModel
        {
            Categories = await _api.GetCategoriesAsync(ct),
            Priorities = await _api.GetPrioritiesAsync(ct)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTicketViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = await _api.GetCategoriesAsync(ct);
            vm.Priorities = await _api.GetPrioritiesAsync(ct);
            return View(vm);
        }

        try
        {
            var id = await _api.CreateTicketAsync(new CreateTicketDto
            {
                Subject = vm.Subject,
                Description = vm.Description,
                CategoryId = vm.CategoryId,
                PriorityId = vm.PriorityId
            }, ct);

            TempData["Success"] = "Заявка успешно зарегистрирована";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            vm.Categories = await _api.GetCategoriesAsync(ct);
            vm.Priorities = await _api.GetPrioritiesAsync(ct);
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, UpdateStatusDto dto, CancellationToken ct)
    {
        try
        {
            await _api.UpdateTicketStatusAsync(id, dto, ct);
            TempData["Success"] = "Статус заявки изменён";
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int id, CreateCommentDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
        {
            TempData["Error"] = "Текст комментария не может быть пустым";
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            await _api.AddCommentAsync(id, dto, ct);
            TempData["Success"] = "Комментарий добавлен";
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}