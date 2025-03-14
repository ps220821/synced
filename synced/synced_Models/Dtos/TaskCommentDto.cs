using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_Models.Dtos
{
    public class TaskCommentDto
    {
        public int user_id { get; set; }
        public int task_id { get; set; }
        public string comment { get; set; }
        public DateTime created_at { get; set; }
    }
}
