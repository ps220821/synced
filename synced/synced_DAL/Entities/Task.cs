using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DAL.Entities
{
    public enum Priorities
    {
        high,
        medium,
        low,
    }
    public enum Status
    {
        todo,
        inprogress,
        done
    }

    public class Task
    {
        public int id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public Status status { get; set; }

        public Priorities priorities { get; set; }

        public DateTime deadline { get; set; }

        public int user_id { get; set; }

        public User? user { get; set; }

        public int project_id { get; set; }

        public Project project { get; set; }

        public ICollection<Project_users> tasks { get; set; }

        public ICollection<TaskComment> taskComments { get; set; }
    }
}
