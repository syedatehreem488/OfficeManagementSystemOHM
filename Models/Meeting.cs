namespace OfficeManagementSystem.Models
{
    public class Meeting
    {
        public int MeetingID { get; set; }
        public string Title { get; set; }
        public DateTime MeetingDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int OrganizerID { get; set; }
    }
}
