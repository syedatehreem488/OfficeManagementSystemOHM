using MySql.Data.MySqlClient;
using OfficeManagementSystem.Models;

namespace OfficeManagementSystem.Data
{
    public class MeetingRepository
    {
        private readonly string _connectionString;

        public MeetingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        public Meeting GetMeetingById(int id)
        {
            Meeting meeting = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Meetings WHERE MeetingID=@MeetingID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MeetingID", id);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            meeting = new Meeting
                            {
                                MeetingID = Convert.ToInt32(reader["MeetingID"]),
                                Title = reader["Title"].ToString(),
                                MeetingDate = Convert.ToDateTime(reader["MeetingDate"]),
                                Location = reader["Location"].ToString(),
                                Description = reader["Description"].ToString(),
                                OrganizerID = Convert.ToInt32(reader["OrganizerID"])
                            };
                        }
                    }
                }
            }

            return meeting;
        }

        public void AddMeeting(Meeting meeting)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
        INSERT INTO Meetings
        (Title, MeetingDate, Location, Description, OrganizerID)
        VALUES
        (@Title,@MeetingDate,@Location,@Description,@OrganizerID)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", meeting.Title);
                    cmd.Parameters.AddWithValue("@MeetingDate", meeting.MeetingDate);
                    cmd.Parameters.AddWithValue("@Location", meeting.Location);
                    cmd.Parameters.AddWithValue("@Description", meeting.Description);
                    cmd.Parameters.AddWithValue("@OrganizerID", meeting.OrganizerID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateMeeting(Meeting meeting)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
        UPDATE Meetings
        SET Title=@Title,
            MeetingDate=@MeetingDate,
            Location=@Location,
            Description=@Description,
            OrganizerID=@OrganizerID
        WHERE MeetingID=@MeetingID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MeetingID", meeting.MeetingID);
                    cmd.Parameters.AddWithValue("@Title", meeting.Title);
                    cmd.Parameters.AddWithValue("@MeetingDate", meeting.MeetingDate);
                    cmd.Parameters.AddWithValue("@Location", meeting.Location);
                    cmd.Parameters.AddWithValue("@Description", meeting.Description);
                    cmd.Parameters.AddWithValue("@OrganizerID", meeting.OrganizerID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteMeeting(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "DELETE FROM Meetings WHERE MeetingID=@MeetingID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MeetingID", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        // Get all meetings
        public List<Meeting> GetAllMeetings()
        {
            List<Meeting> meetings = new List<Meeting>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Meetings";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        meetings.Add(new Meeting
                        {
                            MeetingID = Convert.ToInt32(reader["MeetingID"]),
                            Title = reader["Title"].ToString(),
                            MeetingDate = Convert.ToDateTime(reader["MeetingDate"]),
                            Location = reader["Location"].ToString(),
                            Description = reader["Description"].ToString(),
                            OrganizerID = Convert.ToInt32(reader["OrganizerID"])
                        });
                    }
                }
            }

            return meetings;
        }

        // Add participant to meeting
        public void AddParticipant(int meetingId, int employeeId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO MeetingParticipants
                    (MeetingID, EmployeeID)
                    VALUES
                    (@MeetingID, @EmployeeID)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MeetingID", meetingId);
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}