using MySql.Data.MySqlClient;
using OfficeManagementSystem.Models;
using System.Data;

namespace OfficeManagementSystem.Data
{
    public class PayrollRepository
    {
        private readonly string _connectionString;

        public PayrollRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        // Get all payroll records
        public List<Payroll> GetAllPayroll()
        {
            List<Payroll> payrolls = new List<Payroll>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Payroll";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        payrolls.Add(new Payroll
                        {
                            PayrollID = Convert.ToInt32(reader["PayrollID"]),
                            EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                            Bonus = Convert.ToInt32(reader["Bonus"]),
                            Deduction = Convert.ToInt32(reader["Deduction"]),
                            NetSalary = Convert.ToInt32(reader["NetSalary"]),
                            PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }

            return payrolls;
        }

        // Employee: get only their own payroll records
        public List<Payroll> GetPayrollByEmployee(int employeeId)
        {
            List<Payroll> payrolls = new List<Payroll>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Payroll WHERE EmployeeID = @EmployeeID ORDER BY PaymentDate DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payrolls.Add(new Payroll
                            {
                                PayrollID = Convert.ToInt32(reader["PayrollID"]),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                Bonus = Convert.ToInt32(reader["Bonus"]),
                                Deduction = Convert.ToInt32(reader["Deduction"]),
                                NetSalary = Convert.ToInt32(reader["NetSalary"]),
                                PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }

            return payrolls;
        }

        // Report: payroll filtered by an optional payment-date range and optional employee.
        public List<Payroll> GetPayrollFiltered(DateTime? start, DateTime? end, int? employeeId)
        {
            List<Payroll> payrolls = new List<Payroll>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Payroll WHERE 1 = 1";
                if (start.HasValue) query += " AND PaymentDate >= @Start";
                if (end.HasValue) query += " AND PaymentDate <= @End";
                if (employeeId.HasValue && employeeId.Value > 0) query += " AND EmployeeID = @EmployeeID";
                query += " ORDER BY PaymentDate DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    if (start.HasValue) cmd.Parameters.AddWithValue("@Start", start.Value.Date);
                    if (end.HasValue) cmd.Parameters.AddWithValue("@End", end.Value.Date);
                    if (employeeId.HasValue && employeeId.Value > 0) cmd.Parameters.AddWithValue("@EmployeeID", employeeId.Value);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payrolls.Add(new Payroll
                            {
                                PayrollID = Convert.ToInt32(reader["PayrollID"]),
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                Bonus = Convert.ToInt32(reader["Bonus"]),
                                Deduction = Convert.ToInt32(reader["Deduction"]),
                                NetSalary = Convert.ToInt32(reader["NetSalary"]),
                                PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                Status = reader["Status"].ToString()
                            });
                        }
                    }
                }
            }

            return payrolls;
        }

        // Call CalculatePayroll stored procedure
        public void ProcessPayroll(
            int employeeId,
            int bonus,
            int deduction,
            DateTime paymentDate)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                using (MySqlCommand cmd = new MySqlCommand("CalculatePayroll", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("in_EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("in_Bonus", bonus);
                    cmd.Parameters.AddWithValue("in_Deduction", deduction);
                    cmd.Parameters.AddWithValue("in_PaymentDate", paymentDate);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}