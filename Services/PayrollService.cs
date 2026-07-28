using MySql.Data.MySqlClient;
using OfficeManagementSystem.Data;
using OfficeManagementSystem.Models;
using OfficeManagementSystem.Utilities;

namespace OfficeManagementSystem.Services
{
    public class PayrollService
    {
        private readonly DatabaseHelper _databaseHelper;

        public PayrollService(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }

        public void ProcessPayroll(int employeeID, int bonus, int deduction, DateTime paymentDate)
        {
            try
            {
                if (!ValidationCheck.Bonus(bonus))
                    throw new Exception("Bonus cannot be negative");
                if (!ValidationCheck.Deduction(deduction))
                    throw new Exception("Deduction cannot be negative");

                using (var conn = _databaseHelper.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("CalculatePayroll", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("in_EmployeeID", employeeID);
                    cmd.Parameters.AddWithValue("in_Bonus", bonus);
                    cmd.Parameters.AddWithValue("in_Deduction", deduction);
                    cmd.Parameters.AddWithValue("in_PaymentDate", paymentDate);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                throw;
            }
        }

        public List<Payroll> GetAllPayroll()
        {
            List<Payroll> payrolls = new List<Payroll>();
            try
            {
                using (var conn = _databaseHelper.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM PayrollSummary", conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        payrolls.Add(new Payroll
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Bonus = reader.GetInt32("Bonus"),
                            Deduction = reader.GetInt32("Deduction"),
                            NetSalary = reader.GetInt32("NetSalary"),
                            PaymentDate = reader.GetDateTime("PaymentDate"),
                            Status = reader.GetString("Status")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.SaveError(ex.Message);
                throw;
            }
            return payrolls;
        }
    }
}
