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
        Task<int> CreateAsync(TaskComment taskComment);
        Task<List<TaskComment>> GetAllAsync(int taskId);
    }
    
}
