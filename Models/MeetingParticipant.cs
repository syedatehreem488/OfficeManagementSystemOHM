using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class MeetingParticipant
    {
        public int ParticipantID { get; set; }
        public int MeetingID { get; set; }
        public int EmployeeID { get; set; }
    }
}
