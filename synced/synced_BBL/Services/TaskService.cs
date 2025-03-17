using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = synced_DAL.Entities.Task;


namespace synced_BBL.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService (ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public List<Task> GetAllTasks(int projectId)
        {
            return this._taskRepository.GetAllAsync(projectId);
        }

        public bool CreateTask(TaskDto task)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTask(int id)
        {
            throw new NotImplementedException();
        }

       

        public bool UpdateTask(TaskDto task)
        {
            throw new NotImplementedException();
        }
    }
}
