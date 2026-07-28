namespace OfficeManagementSystem.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime DOB { get; set; }
        public string Status { get; set; }
        public int DepartmentID { get; set; }
        public int RoleID { get; set; }
    }
}
