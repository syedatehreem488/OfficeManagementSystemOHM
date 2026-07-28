namespace OfficeManagementSystem.Models
{
    public class AttendanceSummary
    {
        public int EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public int TotalPresent { get; set; }

        public int TotalAbsent { get; set; }

        public int TotalLate { get; set; }

        public int PendingLeaves { get; set; }
    }
}