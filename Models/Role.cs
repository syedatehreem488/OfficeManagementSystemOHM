using System;
using MySql.Data.MySqlClient;

namespace OfficeManagementSystem.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
