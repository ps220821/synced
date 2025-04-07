using synced_DAL.Entities;
using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Interfaces
{
    public interface ItaskCommentRepository
    {
        int CreateAsync(TaskComment taskComment);
        List<TaskCommentExtended> GetAllAsync(int taskId);

    }
}
