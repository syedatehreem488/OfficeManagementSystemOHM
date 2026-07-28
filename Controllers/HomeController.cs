using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Models;
using OfficeManagementSystem.Data;
using System.Diagnostics;

namespace OfficeManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _databaseHelper;

        public HomeController(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") == null)
                return RedirectToAction("Login", "Account");

            bool connected = _databaseHelper.TestConnection();
            ViewBag.ConnectionStatus = connected
                ? "Connected Successfully"
                : "Connection Failed";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
