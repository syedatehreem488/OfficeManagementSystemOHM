using System;

namespace OfficeManagementSystem.Utilities
{
    public class ValidationCheck
    {
        public static bool Email(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }
        public static bool Gender(string gender)
        {
            return gender == "Male" || gender == "Female";
        }
        public static bool EmployeeStatus(string status)
        {
            return status == "Active" || status == "Resigned" || status == "Terminated" || status == "Retired";
        }

        public static bool ActiveStatus(string status)
        {
            return status == "Yes" || status == "No";
        }

        public static bool AttendanceStatus(string status)
        {
            return status == "Present" || status == "Absent" || status == "Late";
        }
        public static bool AllowedDays(int days)
        {
            return days > 0;
        }

        public static bool Dates(DateTime start, DateTime end)
        {
            return start <= end;
        }
        public static bool LeaveStatus(string status)
        {
            return status == "Pending" || status == "Approved" || status == "Rejected";
        }
        public static bool Salary(int amount)
        {
            return amount > 0;
        }
        public static bool Bonus(int bonus)
        {
            return bonus >= 0;
        }
        public static bool Deduction(int deduction)
        {
            return deduction >= 0;
        }
        public static bool NetSalary(int salary)
        {
            return salary > 0;
        }
        public static bool PayrollStatus(string status)
        {
            return status == "Paid" || status == "Pending";
        }
        public static bool ProjectStatus(string status)
        {
            return status == "Ongoing" || status == "Completed";
        }
        public static bool TaskStatus(string status)
        {
            return status == "Pending" || status == "In Progress" || status == "Completed";
        }

        public static bool ExitType(string type)
        {
            return type == "Resigned" || type == "Terminated" || type == "Retired";
        }

        public static bool Empty(string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool Phone(string phone)
        {
            if (phone.Length != 11)
                return false;
            foreach (char c in phone)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }
}
