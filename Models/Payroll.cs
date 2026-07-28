namespace OfficeManagementSystem.Models
{
    public class Payroll
    {
        public int PayrollID { get; set; }
        public int EmployeeID { get; set; }
        public int Bonus { get; set; }
        public int Deduction { get; set; }
        public int NetSalary { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }
    }
}
