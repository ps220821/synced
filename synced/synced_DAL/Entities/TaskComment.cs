using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DAL.Entities
{
    public class TaskComment
    {
        public int id { get; set; }

        // user relation
        public int user_id { get; set; }
        public User user { get; set; }

        // task relation
        public int task_id { get; set; }
        public Task task { get; set; }

        public string comment { get; set; }
        public DateTime created_at { get; set; }
    }
}
