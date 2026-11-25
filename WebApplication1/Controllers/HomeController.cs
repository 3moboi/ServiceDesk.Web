using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Web.Models;

namespace ServiceDesk.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Очищаем сессию
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Login(int userId)
        {
            // Сохраняем ID в сессию
            HttpContext.Session.SetInt32("UserId", userId);

            // Сохраняем имя для отображения (в реальности мы бы брали это из API)
            var userName = userId switch
            {
                1 => "Иванов И.И. (Заявитель)",
                2 => "Петров П.П. (Исполнитель)",
                3 => "Сидоров С.С. (Администратор)",
                _ => "Неизвестный"
            };
            HttpContext.Session.SetString("UserName", userName);

            return RedirectToAction("Index", "Tickets");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
