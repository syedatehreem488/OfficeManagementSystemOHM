using System;

namespace OfficeManagementSystem.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int EmployeeID { get; set; }
        public string IsActive { get; set; }
        public string IsAdmin { get; set; }
    }
}
