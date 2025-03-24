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
        TaskGroupDto GetAllTasks(int projectId);
        bool CreateTask(TaskDto task);
        bool UpdateTask(TaskDto task);
        bool DeleteTask(int id);
    }
}
