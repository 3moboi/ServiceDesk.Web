using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Web.Services;
using ServiceDesk.Web.ViewModels;

namespace ServiceDesk.Web.Controllers;

public class ReportsController : Controller
{
    private readonly IServiceDeskApiClient _api;

    public ReportsController(IServiceDeskApiClient api)
    {
        _api = api;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? from, DateTime? to, CancellationToken ct)
    {
        var fromDate = from ?? DateTime.Today.AddDays(-30);
        var toDate = to ?? DateTime.Today;

        var vm = new ReportViewModel
        {
            From = fromDate,
            To = toDate,
            Report = await _api.GetReportAsync(fromDate, toDate, ct)
        };
        return View(vm);
    }
}