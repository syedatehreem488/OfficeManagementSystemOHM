namespace OfficeManagementSystem.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public TimeSpan ClockInTime { get; set; }

        // Nullable: an employee who has clocked in but not yet clocked out
        // has no clock-out time.
        public TimeSpan? ClockOutTime { get; set; }

        public string Status { get; set; }
    }
}
