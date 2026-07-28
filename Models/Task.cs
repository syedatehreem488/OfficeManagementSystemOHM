using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class Task
    {
        public int TaskID { get; set; }
        public int ProjectID { get; set; }
        public int EmployeeID { get; set; }
        public string TaskTitle { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}
