using MySql.Data.MySqlClient;
using OfficeManagementSystem.Models;

namespace OfficeManagementSystem.Data
{
    public class AttendanceRepository
    {
        private readonly string _connectionString;

        public AttendanceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        // Admin: get all attendance records
        public List<Attendance> GetAllAttendance()
        {
            List<Attendance> attendanceList = new List<Attendance>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM Attendance ORDER BY AttendanceDate DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        attendanceList.Add(MapAttendance(reader));
                    }
                }
            }

            return attendanceList;
        }

        // Employee: get only their own attendance records
        public List<Attendance> GetAttendanceByEmployee(int employeeId)
        {
            List<Attendance> attendanceList = new List<Attendance>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT *
                                 FROM Attendance
                                 WHERE EmployeeID = @EmployeeID
                                 ORDER BY AttendanceDate DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            attendanceList.Add(MapAttendance(reader));
                        }
                    }
                }
            }

            return attendanceList;
        }

        // Employee: fetch today's attendance row (null if not clocked in yet)
        public Attendance GetTodayAttendance(int employeeId)
        {
            Attendance attendance = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT *
                                 FROM Attendance
                                 WHERE EmployeeID = @EmployeeID
                                 AND DATE(AttendanceDate) = @Today
                                 LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            attendance = MapAttendance(reader);
                        }
                    }
                }
            }

            return attendance;
        }

        // Employee: clock in for today. Status is Late if clocking in after 09:15.
        public void ClockIn(int employeeId)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            string status = now > new TimeSpan(9, 15, 0) ? "Late" : "Present";

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO Attendance
                                 (EmployeeID, AttendanceDate, ClockInTime, Status)
                                 VALUES
                                 (@EmployeeID, @AttendanceDate, @ClockInTime, @Status)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@AttendanceDate", DateTime.Today);
                    cmd.Parameters.AddWithValue("@ClockInTime", now);
                    cmd.Parameters.AddWithValue("@Status", status);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Employee: clock out for today.
        public void ClockOut(int employeeId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE Attendance
                                 SET ClockOutTime = @ClockOutTime
                                 WHERE EmployeeID = @EmployeeID
                                 AND DATE(AttendanceDate) = @Today";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClockOutTime", DateTime.Now.TimeOfDay);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Report: attendance filtered by an optional date range and optional employee.
        public List<Attendance> GetAttendanceFiltered(DateTime? start, DateTime? end, int? employeeId)
        {
            List<Attendance> list = new List<Attendance>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Attendance WHERE 1 = 1";
                if (start.HasValue) query += " AND AttendanceDate >= @Start";
                if (end.HasValue) query += " AND AttendanceDate <= @End";
                if (employeeId.HasValue && employeeId.Value > 0) query += " AND EmployeeID = @EmployeeID";
                query += " ORDER BY AttendanceDate DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (start.HasValue) cmd.Parameters.AddWithValue("@Start", start.Value.Date);
                    if (end.HasValue) cmd.Parameters.AddWithValue("@End", end.Value.Date);
                    if (employeeId.HasValue && employeeId.Value > 0) cmd.Parameters.AddWithValue("@EmployeeID", employeeId.Value);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapAttendance(reader));
                        }
                    }
                }
            }

            return list;
        }

        private Attendance MapAttendance(MySqlDataReader reader)
        {
            return new Attendance
            {
                AttendanceID = Convert.ToInt32(reader["AttendanceID"]),
                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                AttendanceDate = Convert.ToDateTime(reader["AttendanceDate"]),
                ClockInTime = ParseTime(reader["ClockInTime"]),
                ClockOutTime = ParseNullableTime(reader["ClockOutTime"]),
                Status = reader["Status"].ToString()
            };
        }

        private static TimeSpan ParseTime(object value)
        {
            if (value == null || value == DBNull.Value) return TimeSpan.Zero;
            if (value is TimeSpan ts) return ts;
            return TimeSpan.Parse(value.ToString());
        }

        private static TimeSpan? ParseNullableTime(object value)
        {
            if (value == null || value == DBNull.Value) return null;
            if (value is TimeSpan ts) return ts;
            return TimeSpan.Parse(value.ToString());
        }
    }
}
