using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Dtos
{
    public class TaskDto
    {
        public string title { get; set; }

        public string description { get; set; }

        public Status status { get; set; }

        public Priorities priorities { get; set; }

        public DateTime deadline { get; set; }

        public int user_id { get; set; }

        public int project_id { get; set; }

    }
}
