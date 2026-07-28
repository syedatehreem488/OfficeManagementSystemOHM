using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class EmployeeProject
    {
        public int AssignmentID { get; set; }
        public int EmployeeID { get; set; }
        public int ProjectID { get; set; }
        public DateTime AssignDate { get; set; }
    }
}
