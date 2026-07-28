using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;
using OfficeManagementSystem.Models;
using OfficeManagementSystem.Utilities;

namespace OfficeManagementSystem.Controllers
{
    public class LeaveController : Controller
    {
        private readonly LeaveRepository _leaveRepository;

        public LeaveController(LeaveRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";
        private int GetEmployeeID() => HttpContext.Session.GetInt32("EmployeeID") ?? 0;

        // Employee: view own leaves + submit form
        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            if (IsAdmin()) return RedirectToAction("Admin");

            var myLeaves = _leaveRepository.GetLeavesByEmployee(GetEmployeeID());
            return View(myLeaves);
        }

        // Employee: submit new leave request
        [HttpPost]
        public IActionResult Submit(LeaveRequest leave)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            // Validate input with ValidationCheck before the database
            if (leave.LeaveTypeID <= 0)
            {
                TempData["Error"] = "Please select a leave type.";
                return RedirectToAction("Index");
            }
            if (ValidationCheck.Empty(leave.Reason))
            {
                TempData["Error"] = "Please provide a reason for your leave.";
                return RedirectToAction("Index");
            }
            if (!ValidationCheck.Dates(leave.StartDate, leave.EndDate))
            {
                TempData["Error"] = "Start date must be on or before the end date.";
                return RedirectToAction("Index");
            }

            leave.EmployeeID = GetEmployeeID();

            try
            {
                _leaveRepository.SubmitLeave(leave);
                TempData["Success"] = "Leave request submitted.";
            }
            catch (Exception ex)
            {
                // This also catches the CheckLeaveDays trigger if the request is too long.
                Logging.SaveError(ex.Message);
                TempData["Error"] = "Could not submit the request. It may exceed the allowed number of days for that leave type.";
            }

            return RedirectToAction("Index");
        }

        // Admin: view all leaves + approve/reject
        public IActionResult Admin()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            var allLeaves = _leaveRepository.GetAllLeaves();
            return View(allLeaves);
        }

        // Admin: approve a leave
        public IActionResult Approve(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            try { _leaveRepository.ApproveLeave(id, "Approved"); }
            catch (Exception ex) { Logging.SaveError(ex.Message); TempData["Error"] = "Could not update the leave request."; }

            return RedirectToAction("Admin");
        }

        // Admin: reject a leave
        public IActionResult Reject(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            try { _leaveRepository.ApproveLeave(id, "Rejected"); }
            catch (Exception ex) { Logging.SaveError(ex.Message); TempData["Error"] = "Could not update the leave request."; }

            return RedirectToAction("Admin");
        }
    }
}
