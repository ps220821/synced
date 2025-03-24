using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DAL.Entities
{
    public enum Roles
    {
        admin,
        member,
        viewer
    }

    public class Project_users
    {
        public int id { get; set; }

        public int project_id { get; set; }
        public Project project { get; set; }
        public int user_id { get; set; }
        public Roles roles { get; set; }
    }
}
