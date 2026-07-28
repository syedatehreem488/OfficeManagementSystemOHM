using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;
using OfficeManagementSystem.Utilities;

namespace OfficeManagementSystem.Controllers
{
    public class PayrollController : Controller
    {
        private readonly PayrollRepository _payrollRepository;

        public PayrollController(PayrollRepository payrollRepository)
        {
            _payrollRepository = payrollRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";
        private int GetEmployeeID() => HttpContext.Session.GetInt32("EmployeeID") ?? 0;

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            // Admin sees all payroll; an employee sees only their own.
            var payrolls = IsAdmin()
                ? _payrollRepository.GetAllPayroll()
                : _payrollRepository.GetPayrollByEmployee(GetEmployeeID());

            return View(payrolls);
        }

        [HttpPost]
        public IActionResult Process(int employeeId, int bonus, int deduction, DateTime paymentDate)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            // Validate with ValidationCheck before the database
            if (employeeId <= 0)
            {
                TempData["Error"] = "Please enter a valid employee ID.";
                return RedirectToAction("Index");
            }
            if (!ValidationCheck.Bonus(bonus))
            {
                TempData["Error"] = "Bonus cannot be negative.";
                return RedirectToAction("Index");
            }
            if (!ValidationCheck.Deduction(deduction))
            {
                TempData["Error"] = "Deduction cannot be negative.";
                return RedirectToAction("Index");
            }

            try
            {
                _payrollRepository.ProcessPayroll(employeeId, bonus, deduction, paymentDate);
                TempData["Success"] = "Payroll processed.";
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                TempData["Error"] = "Could not process payroll. Make sure the employee has a salary record.";
            }

            return RedirectToAction("Index");
        }
    }
}
