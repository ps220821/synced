using synced.Core.Results;
using synced_BBL.Dtos;
using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Interfaces
{
    public interface ITaskCommentService
    {
        Task<OperationResult<int>> AddComment(TaskCommentDto taskComment);
        Task<OperationResult<List<TaskCommentExtendedDto>>> GetTaskComments(int taskId);

    }
}
