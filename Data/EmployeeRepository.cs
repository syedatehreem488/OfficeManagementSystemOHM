using MySql.Data.MySqlClient;
using OfficeManagementSystem.Models;

namespace OfficeManagementSystem.Data
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        public void AddEmployee(Employee emp)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO Employees
                        (FirstName, LastName, Email, Phone, Gender, DOB, HireDate, Status, DepartmentID, RoleID)
                        VALUES
                        (@FirstName, @LastName, @Email, @Phone, @Gender, @DOB, @HireDate, @Status, @DepartmentID, @RoleID)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@DOB", emp.DOB);
                    cmd.Parameters.AddWithValue("@HireDate", emp.HireDate);
                    cmd.Parameters.AddWithValue("@Status", emp.Status);
                    cmd.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);
                    cmd.Parameters.AddWithValue("@RoleID", emp.RoleID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Employee GetEmployeeById(int id)
        {
            Employee employee = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Employees WHERE EmployeeID=@EmployeeID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            employee = new Employee
                            {
                                EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                DOB = Convert.ToDateTime(reader["DOB"]),
                                HireDate = Convert.ToDateTime(reader["HireDate"]),
                                Status = reader["Status"].ToString(),
                                DepartmentID = Convert.ToInt32(reader["DepartmentID"]),
                                RoleID = Convert.ToInt32(reader["RoleID"])
                            };
                        }
                    }
                }
            }

            return employee;
        }

        public void UpdateEmployee(Employee emp)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE Employees
                         SET FirstName=@FirstName,
                             LastName=@LastName,
                             Email=@Email,
                             Phone=@Phone,
                             Gender=@Gender,
                             DOB=@DOB,
                             HireDate=@HireDate,
                             Status=@Status,
                             DepartmentID=@DepartmentID,
                             RoleID=@RoleID
                         WHERE EmployeeID=@EmployeeID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                    cmd.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", emp.LastName);
                    cmd.Parameters.AddWithValue("@Email", emp.Email);
                    cmd.Parameters.AddWithValue("@Phone", emp.Phone);
                    cmd.Parameters.AddWithValue("@Gender", emp.Gender);
                    cmd.Parameters.AddWithValue("@DOB", emp.DOB);
                    cmd.Parameters.AddWithValue("@HireDate", emp.HireDate);
                    cmd.Parameters.AddWithValue("@Status", emp.Status);
                    cmd.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);
                    cmd.Parameters.AddWithValue("@RoleID", emp.RoleID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteEmployee(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "DELETE FROM Employees WHERE EmployeeID=@EmployeeID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Employee> GetAllEmployees()
        {
            List<Employee> employees = new();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Employees";

                using var cmd = new MySqlCommand(query, conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Status = reader["Status"].ToString()
                    });
                }
            }

            return employees;
        }
    }
}
