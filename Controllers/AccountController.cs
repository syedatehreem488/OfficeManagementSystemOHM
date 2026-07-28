using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Fetch user + their role name via join
                string query = @"SELECT u.UserID, u.UserName, u.EmployeeID, u.IsAdmin
                                 FROM Users u
                                 WHERE u.UserName = @username
                                 AND u.Password = @password
                                 AND u.IsActive = 'Yes'";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Store identity in session
                    HttpContext.Session.SetString("UserName", reader["UserName"].ToString());
                    HttpContext.Session.SetInt32("EmployeeID", Convert.ToInt32(reader["EmployeeID"]));
                    HttpContext.Session.SetString("IsAdmin", reader["IsAdmin"].ToString());

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid username or password, or account is inactive.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
