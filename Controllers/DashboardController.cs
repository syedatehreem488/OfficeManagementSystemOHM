using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;

namespace OfficeManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardRepository _dashboardRepository;

        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            // The dashboard is an organisation-wide management summary — admin only.
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var summary = _dashboardRepository.GetAttendanceSummary();
            return View(summary);
        }
    }
}
