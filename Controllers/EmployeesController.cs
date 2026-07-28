using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;
using OfficeManagementSystem.Models;
using OfficeManagementSystem.Utilities;

namespace OfficeManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeesController(EmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";

        // Runs every ValidationCheck rule that applies to an employee.
        // Returns an error message, or null if everything is valid.
        private string ValidateEmployee(Employee emp)
        {
            if (ValidationCheck.Empty(emp.FirstName) || ValidationCheck.Empty(emp.LastName))
                return "First name and last name are required.";
            if (ValidationCheck.Empty(emp.Email) || !ValidationCheck.Email(emp.Email))
                return "Please enter a valid email address (it must contain @ and a dot).";
            if (ValidationCheck.Empty(emp.Phone) || !ValidationCheck.Phone(emp.Phone))
                return "Phone must be exactly 11 digits and contain numbers only.";
            if (ValidationCheck.Empty(emp.Gender) || !ValidationCheck.Gender(emp.Gender))
                return "Gender must be Male or Female.";
            if (ValidationCheck.Empty(emp.Status) || !ValidationCheck.EmployeeStatus(emp.Status))
                return "Status must be Active, Resigned, Terminated or Retired.";
            if (!ValidationCheck.Dates(emp.DOB, emp.HireDate))
                return "Date of birth must be on or before the hire date.";
            return null;
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            // Employee management is an admin function.
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var employees = _employeeRepository.GetAllEmployees();
            return View(employees);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee emp)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            // 1. Validate input with ValidationCheck BEFORE going to the database.
            string error = ValidateEmployee(emp);
            if (error != null)
            {
                ViewBag.Error = error;
                return View(emp);   // re-show the form with the entered values
            }

            // 2. Database operation guarded by try/catch + logging.
            try
            {
                _employeeRepository.AddEmployee(emp);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                ViewBag.Error = "Could not save the employee. Please check the values and try again.";
                return View(emp);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            var employee = _employeeRepository.GetEmployeeById(id);
            return View(employee);
        }

        [HttpPost]
        public IActionResult Edit(Employee emp)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            string error = ValidateEmployee(emp);
            if (error != null)
            {
                ViewBag.Error = error;
                return View(emp);
            }

            try
            {
                _employeeRepository.UpdateEmployee(emp);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                ViewBag.Error = "Could not update the employee. Please check the values and try again.";
                return View(emp);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            try
            {
                _employeeRepository.DeleteEmployee(id);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                TempData["Error"] = "Could not delete the employee.";
            }

            return RedirectToAction("Index");
        }
    }
}
