using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class Salary
    {
        public int SalaryID { get; set; }
        public int EmployeeID { get; set; }
        public int BasicSalary { get; set; }
        public DateTime SalaryAssignDate { get; set; }
    }
}
