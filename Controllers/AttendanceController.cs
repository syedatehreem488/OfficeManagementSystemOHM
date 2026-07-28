using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;

namespace OfficeManagementSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AttendanceRepository _attendanceRepository;

        public AttendanceController(AttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";
        private int GetEmployeeID() => HttpContext.Session.GetInt32("EmployeeID") ?? 0;

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (IsAdmin())
            {
                // Admin sees every employee's attendance.
                var all = _attendanceRepository.GetAllAttendance();
                return View(all);
            }

            // Employee sees only their own records, plus today's clock-in/out state.
            int empId = GetEmployeeID();
            ViewBag.Today = _attendanceRepository.GetTodayAttendance(empId);
            var mine = _attendanceRepository.GetAttendanceByEmployee(empId);
            return View(mine);
        }

        // Employee: clock in for today
        [HttpPost]
        public IActionResult ClockIn()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int empId = GetEmployeeID();
            var today = _attendanceRepository.GetTodayAttendance(empId);

            // Only clock in once per day
            if (today == null)
                _attendanceRepository.ClockIn(empId);

            return RedirectToAction("Index");
        }

        // Employee: clock out for today
        [HttpPost]
        public IActionResult ClockOut()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            int empId = GetEmployeeID();
            var today = _attendanceRepository.GetTodayAttendance(empId);

            // Can only clock out if clocked in and not already clocked out
            if (today != null && today.ClockOutTime == null)
                _attendanceRepository.ClockOut(empId);

            return RedirectToAction("Index");
        }
    }
}
