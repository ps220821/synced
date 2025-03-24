using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Dtos
{
    public class TaskGroupDto
    {
        public List<TaskDto> TodoTasks { get; set; }
        public List<TaskDto> InProgressTasks { get; set; }
        public List<TaskDto> DoneTasks { get; set; }
    }
}
