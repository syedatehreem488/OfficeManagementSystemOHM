using Microsoft.AspNetCore.Mvc;
using OfficeManagementSystem.Data;
using OfficeManagementSystem.Models;
using OfficeManagementSystem.Utilities;

namespace OfficeManagementSystem.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly MeetingRepository _meetingRepository;

        public MeetingsController(MeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        private bool IsLoggedIn() => HttpContext.Session.GetString("UserName") != null;
        private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "Yes";

        private string ValidateMeeting(Meeting meeting)
        {
            if (ValidationCheck.Empty(meeting.Title))
                return "Meeting title is required.";
            if (ValidationCheck.Empty(meeting.Location))
                return "Location is required.";
            if (meeting.OrganizerID <= 0)
                return "A valid organizer ID is required.";
            return null;
        }

        public IActionResult Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");

            var meetings = _meetingRepository.GetAllMeetings();
            return View(meetings);
        }

        public IActionResult Create()
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Meeting meeting)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            string error = ValidateMeeting(meeting);
            if (error != null)
            {
                ViewBag.Error = error;
                return View(meeting);
            }

            try
            {
                _meetingRepository.AddMeeting(meeting);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                ViewBag.Error = "Could not save the meeting. Please check the values and try again.";
                return View(meeting);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            var meeting = _meetingRepository.GetMeetingById(id);
            return View(meeting);
        }

        [HttpPost]
        public IActionResult Edit(Meeting meeting)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            string error = ValidateMeeting(meeting);
            if (error != null)
            {
                ViewBag.Error = error;
                return View(meeting);
            }

            try
            {
                _meetingRepository.UpdateMeeting(meeting);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                ViewBag.Error = "Could not update the meeting. Please check the values and try again.";
                return View(meeting);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("Login", "Account");
            if (!IsAdmin()) return RedirectToAction("Index");

            try
            {
                _meetingRepository.DeleteMeeting(id);
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                TempData["Error"] = "Could not delete the meeting.";
            }

            return RedirectToAction("Index");
        }
    }
}
