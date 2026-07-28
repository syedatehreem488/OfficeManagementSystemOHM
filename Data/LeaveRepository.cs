using MySql.Data.MySqlClient;
using OfficeManagementSystem.Models;

namespace OfficeManagementSystem.Data
{
    public class LeaveRepository
    {
        private readonly string _connectionString;

        public LeaveRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        // Admin: get all leave requests
        public List<LeaveRequest> GetAllLeaves()
        {
            List<LeaveRequest> leaves = new List<LeaveRequest>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM LeaveRequests ORDER BY StartDate DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        leaves.Add(MapLeave(reader));
                    }
                }
            }

            return leaves;
        }

        // Employee: get only their own leave requests
        public List<LeaveRequest> GetLeavesByEmployee(int employeeId)
        {
            List<LeaveRequest> leaves = new List<LeaveRequest>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM LeaveRequests
                                 WHERE EmployeeID = @EmployeeID
                                 ORDER BY StartDate DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            leaves.Add(MapLeave(reader));
                        }
                    }
                }
            }

            return leaves;
        }

        // Employee: submit a new leave request
        public void SubmitLeave(LeaveRequest leave)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO LeaveRequests
                                 (EmployeeID, LeaveTypeID, Reason, StartDate, EndDate, Approved)
                                 VALUES
                                 (@EmployeeID, @LeaveTypeID, @Reason, @StartDate, @EndDate, 'Pending')";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", leave.EmployeeID);
                    cmd.Parameters.AddWithValue("@LeaveTypeID", leave.LeaveTypeID);
                    cmd.Parameters.AddWithValue("@Reason", leave.Reason);
                    cmd.Parameters.AddWithValue("@StartDate", leave.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", leave.EndDate);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Admin: call stored procedure ApproveLeave
        public void ApproveLeave(int leaveId, string status)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand("ApproveLeave", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("in_LeaveID", leaveId);
                    cmd.Parameters.AddWithValue("in_Status", status);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private LeaveRequest MapLeave(MySqlDataReader reader)
        {
            return new LeaveRequest
            {
                LeaveID = Convert.ToInt32(reader["LeaveID"]),
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                LeaveTypeID = Convert.ToInt32(reader["LeaveTypeID"]),
                Reason = reader["Reason"].ToString(),
                StartDate = Convert.ToDateTime(reader["StartDate"]),
                EndDate = Convert.ToDateTime(reader["EndDate"]),
                Approved = reader["Approved"].ToString()
            };
        }
    }
}
