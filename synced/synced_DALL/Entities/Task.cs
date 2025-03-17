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
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; }

        public Priorities Priority { get; set; }

        public DateTime Deadline { get; set; }

        public int User_id { get; set; }

        public int Project_id { get; set; }


    }
}
