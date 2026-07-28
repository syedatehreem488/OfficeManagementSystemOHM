using MySql.Data.MySqlClient;
using OfficeManagementSystem.Models;

namespace OfficeManagementSystem.Data
{
    public class ProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        public Project GetProjectById(int id)
        {
            Project project = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Projects WHERE ProjectID=@ProjectID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            project = new Project
                            {
                                ProjectID = Convert.ToInt32(reader["ProjectID"]),
                                ProjectName = reader["ProjectName"].ToString(),
                                Description = reader["Description"].ToString(),
                                StartDate = Convert.ToDateTime(reader["StartDate"]),
                                EndDate = Convert.ToDateTime(reader["EndDate"]),
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
            }

            return project;
        }

        public void AddProject(Project project)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
        INSERT INTO Projects
        (ProjectName, Description, StartDate, EndDate, Status)
        VALUES
        (@ProjectName,@Description,@StartDate,@EndDate,@Status)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                    cmd.Parameters.AddWithValue("@Description", project.Description);
                    cmd.Parameters.AddWithValue("@StartDate", project.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", project.EndDate);
                    cmd.Parameters.AddWithValue("@Status", project.Status);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProject(Project project)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
        UPDATE Projects
        SET ProjectName=@ProjectName,
            Description=@Description,
            StartDate=@StartDate,
            EndDate=@EndDate,
            Status=@Status
        WHERE ProjectID=@ProjectID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", project.ProjectID);
                    cmd.Parameters.AddWithValue("@ProjectName", project.ProjectName);
                    cmd.Parameters.AddWithValue("@Description", project.Description);
                    cmd.Parameters.AddWithValue("@StartDate", project.StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", project.EndDate);
                    cmd.Parameters.AddWithValue("@Status", project.Status);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProject(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "DELETE FROM Projects WHERE ProjectID=@ProjectID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProjectID", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }



        // Get all projects
        public List<Project> GetAllProjects()
        {
            List<Project> projects = new List<Project>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Projects";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        projects.Add(new Project
                        {
                            ProjectID = Convert.ToInt32(reader["ProjectID"]),
                            ProjectName = reader["ProjectName"].ToString(),
                            Description = reader["Description"].ToString(),
                            StartDate = Convert.ToDateTime(reader["StartDate"]),
                            EndDate = Convert.ToDateTime(reader["EndDate"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }

            return projects;
        }

        // Assign employee to project
        public void AssignEmployee(int employeeId, int projectId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO EmployeeProjects
                    (EmployeeID, ProjectID, AssignDate)
                    VALUES
                    (@EmployeeID, @ProjectID, @AssignDate)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@ProjectID", projectId);
                    cmd.Parameters.AddWithValue("@AssignDate", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}