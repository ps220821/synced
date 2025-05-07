using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; }

        public Priorities Priority { get; set; }

        public DateTime Deadline { get; set; }

        public int? UserId { get; set; }

        public int ProjectId { get; set; }

    }
}
