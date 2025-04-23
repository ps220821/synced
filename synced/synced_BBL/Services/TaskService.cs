using Microsoft.Data.SqlClient;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using velocitaApi.Mappers;
using Task = synced_DALL.Entities.Task;


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
                List<Task> tasks = await _taskRepository.GetAllAsync(projectId);

                List<TaskDto> taskDtos = tasks.Select(task => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    Deadline = task.Deadline,
                    ProjectId = task.ProjectId,
                    UserId = task.UserId
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
                if (task == null) return OperationResult<bool>.Failure("Task cannot be null.");

                Task newTask = Mapper.MapCreate<Task>(task);
                int newTaskId = await _taskRepository.CreateAsync(newTask);

                if (newTaskId > 0)
                {
                    return OperationResult<bool>.Success(true); // Successfully created
                }
                else
                {
                    return OperationResult<bool>.Failure("Task could not be created.");
                }
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (SqlException ex)
            {
                return OperationResult<bool>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("An unexpected error occurred while creating the task.");
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
                if (task == null) return OperationResult<bool>.Failure("Task cannot be null.");

                Task updatedTask = Mapper.MapCreate<Task>(task);
                updatedTask.Status = task.Status;
                updatedTask.Priority = task.Priority;

                int rowsAffected = await _taskRepository.UpdateAsync(updatedTask);

                if (rowsAffected > 0)
                {
                    return OperationResult<bool>.Success(true); // Successfully updated
                }
                else
                {
                    return OperationResult<bool>.Failure("Task could not be updated or does not exist.");
                }
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (SqlException ex)
            {
                return OperationResult<bool>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("An unexpected error occurred while updating the task.");
            }

        }
    }
}
