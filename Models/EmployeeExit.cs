using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class EmployeeExit
    {
        public int ExitID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime ExitDate { get; set; }
        public string ExitType { get; set; }
        public string Reason { get; set; }
        public int ProcessedByEmployee { get; set; }
    }
}
