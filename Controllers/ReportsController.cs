using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;
using OfficeManagementSystem.Services;
using OfficeManagementSystem.Utilities;

namespace OfficeManagementSystem.Controllers
{
    // Reports are a management function, so the whole controller is admin-only.
    public class ReportsController : Controller
    {
        private readonly AttendanceRepository _attendanceRepository;
        private readonly PayrollRepository _payrollRepository;

        public ReportsController(AttendanceRepository attendanceRepository, PayrollRepository payrollRepository)
        {
            _attendanceRepository = attendanceRepository;
            _payrollRepository = payrollRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";

        // Landing page that links to the available reports.
        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            return View();
        }

        // ---- Attendance report (parameter-based) ----

        public IActionResult Attendance(DateTime? start, DateTime? end, int? employeeId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (start.HasValue && end.HasValue && !ValidationCheck.Dates(start.Value, end.Value))
            {
                TempData["Error"] = "Start date must be on or before the end date.";
                start = null; end = null;
            }

            ViewBag.Start = start;
            ViewBag.End = end;
            ViewBag.EmployeeId = employeeId;

            var data = _attendanceRepository.GetAttendanceFiltered(start, end, employeeId);
            return View(data);
        }

        public IActionResult AttendancePdf(DateTime? start, DateTime? end, int? employeeId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                var data = _attendanceRepository.GetAttendanceFiltered(start, end, employeeId);
                byte[] pdf = PdfReportService.BuildAttendancePdf(data, start, end, employeeId);
                return File(pdf, "application/pdf", "AttendanceReport.pdf");
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                TempData["Error"] = "Could not generate the PDF.";
                return RedirectToAction("Attendance", new { start, end, employeeId });
            }
        }

        // ---- Payroll report (parameter-based) ----

        public IActionResult Payroll(DateTime? start, DateTime? end, int? employeeId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (start.HasValue && end.HasValue && !ValidationCheck.Dates(start.Value, end.Value))
            {
                TempData["Error"] = "Start date must be on or before the end date.";
                start = null; end = null;
            }

            ViewBag.Start = start;
            ViewBag.End = end;
            ViewBag.EmployeeId = employeeId;

            var data = _payrollRepository.GetPayrollFiltered(start, end, employeeId);
            return View(data);
        }

        public IActionResult PayrollPdf(DateTime? start, DateTime? end, int? employeeId)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            try
            {
                var data = _payrollRepository.GetPayrollFiltered(start, end, employeeId);
                byte[] pdf = PdfReportService.BuildPayrollPdf(data, start, end, employeeId);
                return File(pdf, "application/pdf", "PayrollReport.pdf");
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                TempData["Error"] = "Could not generate the PDF.";
                return RedirectToAction("Payroll", new { start, end, employeeId });
            }
        }
    }
}
