using MySql.Data.MySqlClient;
using OfficeManagementSystem.Models;

namespace OfficeManagementSystem.Data
{
    public class DashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        public List<AttendanceSummary> GetAttendanceSummary()
        {
            List<AttendanceSummary> summary = new List<AttendanceSummary>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM AttendanceSummary";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        summary.Add(new AttendanceSummary
                        {
                            EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                            EmployeeName = reader["EmployeeName"].ToString(),
                            TotalPresent = Convert.ToInt32(reader["TotalPresent"]),
                            TotalAbsent = Convert.ToInt32(reader["TotalAbsent"]),
                            TotalLate = Convert.ToInt32(reader["TotalLate"]),
                            PendingLeaves = Convert.ToInt32(reader["PendingLeaves"])
                        });
                    }
                }
            }

            return summary;
        }
    }
}
