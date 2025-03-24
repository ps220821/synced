using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL.Entities;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using velocitaApi.Mappers;
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
        public TaskGroupDto GetAllTasks(int projectId)
        {
            var tasks = _taskRepository.GetAllAsync(projectId);

            List<TaskDto> taskDtos = tasks.Select(task => new TaskDto
            {
                Id = task.Id,
                Title = task.Title,  // Use 'Title' instead of 'title'
                Description = task.Description,  // Correct property names
                Status = task.Status,  // Correct property name for status
                Priority = task.Priority,  // Correct property name for priorities
                Deadline = task.Deadline,  // Correct property name for deadline
                Project_id = task.Project_id,
                User_id = task.User_id,
            }).ToList();

            return GetTasksGroupedByStatus(taskDtos); ;
        }

        public TaskGroupDto GetTasksGroupedByStatus(List<TaskDto> tasks)
        {
            TaskGroupDto taskGroupDto = new TaskGroupDto();

            if (tasks.Count > 0)
            {
                taskGroupDto.TodoTasks = tasks.Where(tasks => tasks.Status == Status.todo).ToList();
                taskGroupDto.InProgressTasks = tasks.Where(tasks => tasks.Status == Status.inprogress).ToList();
                taskGroupDto.DoneTasks = tasks.Where(tasks => tasks.Status == Status.done).ToList();
            }

            return taskGroupDto;
        }

        public bool CreateTask(TaskDto task)
        {
            Task newTask = Mapper.MapCreate<Task>(task);
            return this._taskRepository.CreateAsync(newTask);
        }

        public bool DeleteTask(int id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateTask(TaskDto task)
        {
            Task newTask = Mapper.MapCreate<Task>(task);
            newTask.Status = task.Status;
            newTask.Priority = task.Priority;
            return this._taskRepository.UpdateAsync(Mapper.MapCreate<Task>(task));
        }
    }
}
