using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DAL.Entities
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateTime created_at { get; set; }

        public ICollection<Project> projects { get; set; }
        public ICollection<Project_users> ProjectUsers { get; set; }
        public ICollection<TaskComment> taskComments { get; set; }

    }
}
