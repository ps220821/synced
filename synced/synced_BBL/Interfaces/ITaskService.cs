using synced.Core.Results;
using synced_BBL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = synced_DAL.Entities.Task;



namespace synced_BBL.Interfaces
{
    public interface ITaskService
    {
        Task<OperationResult<TaskGroupDto>> GetAllTasks(int projectId);  // Updated return type to OperationResult<TaskGroupDto>
        Task<OperationResult<bool>> CreateTask(TaskDto task);  // Updated return type to OperationResult<bool>
        Task<OperationResult<bool>> UpdateTask(TaskDto task);  // Updated return type to OperationResult<bool>
        Task<OperationResult<bool>> DeleteTask(int id);
    }
}
