using Microsoft.Data.SqlClient;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
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
        public async Task<OperationResult<TaskGroupDto>> GetAllTasks(int projectId)
        {
            try
            {
                List<Task> tasks = _taskRepository.GetAllAsync(projectId);

                List<TaskDto> taskDtos = tasks.Select(task => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    Deadline = task.Deadline,
                    Project_id = task.Project_id,
                    User_id = task.User_id,
                }).ToList();

                TaskGroupDto taskGroup = GetTasksGroupedByStatus(taskDtos);

                return OperationResult<TaskGroupDto>.Success(taskGroup);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<TaskGroupDto>.Failure(ex.Message);
            }
            catch (SqlException ex)
            {
                return OperationResult<TaskGroupDto>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<TaskGroupDto>.Failure("An unexpected error occurred while fetching tasks.");
            }

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

        public async Task<OperationResult<bool>> CreateTask(TaskDto task)
        {
            try
            {
                Task newTask = Mapper.MapCreate<Task>(task);

                // Call CreateAsync and await the result
                bool result = this._taskRepository.CreateAsync(newTask);
                if (result)
                {
                    return OperationResult<bool>.Success(true);  // Successfully created
                }
                else
                {
                    return OperationResult<bool>.Failure("Task could not be created.");  // Creation failed
                }
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);  // Handle DatabaseException
            }
            catch (SqlException ex)
            {
                return OperationResult<bool>.Failure(DatabaseHelper.GetErrorMessage(ex));  // Handle SQL errors
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure("An unexpected error occurred while creating the task.");  // Handle other errors
            }

        }

        public Task<OperationResult<bool>> DeleteTask(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<bool>> UpdateTask(TaskDto task)
        {
            try
            {
                Task updatedTask = Mapper.MapCreate<Task>(task);

                // Update the task's status and priority
                updatedTask.Status = task.Status;
                updatedTask.Priority = task.Priority;

                // Call UpdateAsync and await the result
                bool success =  this._taskRepository.UpdateAsync(updatedTask);

                if (success)
                {
                    return OperationResult<bool>.Success(true);  // Successfully updated
                }
                else
                {
                    return OperationResult<bool>.Failure("Task could not be updated.");  // Update failed
                }
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);  // Handle DatabaseException
            }
            catch (SqlException ex)
            {
                return OperationResult<bool>.Failure(DatabaseHelper.GetErrorMessage(ex));  // Handle SQL errors
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure("An unexpected error occurred while updating the task.");  // Handle other errors
            }

        }
    }
}
