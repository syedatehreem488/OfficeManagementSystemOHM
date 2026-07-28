using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;
using OfficeManagementSystem.Models;
using OfficeManagementSystem.Utilities;

namespace OfficeManagementSystem.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectsController(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";

        private string ValidateProject(Project project)
        {
            if (ValidationCheck.Empty(project.ProjectName))
                return "Project name is required.";
            if (!ValidationCheck.Dates(project.StartDate, project.EndDate))
                return "Start date must be on or before the end date.";
            if (ValidationCheck.Empty(project.Status) || !ValidationCheck.ProjectStatus(project.Status))
                return "Status must be Ongoing or Completed.";
            return null;
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var projects = _projectRepository.GetAllProjects();
            return View(projects);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Project project)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            string error = ValidateProject(project);
            if (error != null)
            {
                ViewBag.Error = error;
                return View(project);
            }

            try
            {
                _projectRepository.AddProject(project);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                ViewBag.Error = "Could not save the project. Please check the values and try again.";
                return View(project);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            var project = _projectRepository.GetProjectById(id);
            return View(project);
        }

        [HttpPost]
        public IActionResult Edit(Project project)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            string error = ValidateProject(project);
            if (error != null)
            {
                ViewBag.Error = error;
                return View(project);
            }

            try
            {
                _projectRepository.UpdateProject(project);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                ViewBag.Error = "Could not update the project. Please check the values and try again.";
                return View(project);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            try
            {
                _projectRepository.DeleteProject(id);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                TempData["Error"] = "Could not delete the project.";
            }

            return RedirectToAction("Index");
        }
    }
}
