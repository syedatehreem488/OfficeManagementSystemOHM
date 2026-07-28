using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
    }
}
