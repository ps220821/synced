using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DAL.Entities
{
    public class Project
    {
        public int id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public DateOnly start_date { get; set; }

        public DateOnly end_date { get; private set; }

        public int owner { get; set; }
        public User user { get; set; }




        public ICollection<Task> tasks { get; set; }

        public ICollection<Project_users> ProjectUsers { get; set; }
    }
}
