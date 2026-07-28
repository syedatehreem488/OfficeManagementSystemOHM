using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class LeaveType
    {
        public int LeaveTypeID { get; set; }
        public string LeaveTypeName { get; set; }
        public int AllowedDays { get; set; }
    }
}
