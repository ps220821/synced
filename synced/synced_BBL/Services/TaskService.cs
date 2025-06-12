
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using Task = synced_DALL.Entities.Task;

namespace synced_BBL.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<OperationResult<TaskGroupDto>> GetAllTasks(int projectId)
        {
            try
            {
                List<Task> tasks = await _taskRepository.GetAllAsync(projectId);

                var taskDtos = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    Deadline = t.Deadline,
                    ProjectId = t.ProjectId,
                    UserId = t.UserId
                }).ToList();

                TaskGroupDto taskGroup = GetTasksGroupedByStatus(taskDtos);
                return OperationResult<TaskGroupDto>.Success(taskGroup);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<TaskGroupDto>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<TaskGroupDto>.Failure("An unexpected error occurred while fetching tasks.");
            }
        }

        public TaskGroupDto GetTasksGroupedByStatus(List<TaskDto> tasks)
        {
            return new TaskGroupDto
            {
                TodoTasks = tasks.Where(t => t.Status == Status.todo).ToList(),
                InProgressTasks = tasks.Where(t => t.Status == Status.inprogress).ToList(),
                DoneTasks = tasks.Where(t => t.Status == Status.done).ToList()
            };
        }

        public async Task<OperationResult<bool>> CreateTask(TaskDto taskDto)
        {
            try
            {
                if (taskDto == null)
                    return OperationResult<bool>.Failure("Task cannot be null.");

                Task newTask = Task.Create(
                    taskDto.Title,
                    taskDto.Description,
                    taskDto.Status,
                    taskDto.Priority,
                    taskDto.Deadline,
                    taskDto.UserId,
                    taskDto.ProjectId
                );

                int newTaskId = await _taskRepository.CreateAsync(newTask);
                return (newTaskId > 0)
                    ? OperationResult<bool>.Success(true)
                    : OperationResult<bool>.Failure("Task could not be created.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("An unexpected error occurred while creating the task.");
            }
        }

        public async Task<OperationResult<bool>> UpdateTask(TaskDto taskDto)
        {
            try
            {
                if (taskDto == null)
                    return OperationResult<bool>.Failure("Task cannot be null.");

                Task updatedTask = Task.FromExisting(
                    taskDto.Id,
                    taskDto.Title,
                    taskDto.Description,
                    taskDto.Status,
                    taskDto.Priority,
                    taskDto.Deadline,
                    taskDto.UserId,
                    taskDto.ProjectId
                );

                int rowsAffected = await _taskRepository.UpdateAsync(updatedTask);

                return (rowsAffected > 0)
                    ? OperationResult<bool>.Success(true)
                    : OperationResult<bool>.Failure("Task could not be updated or does not exist.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("An unexpected error occurred while updating the task.");
            }
        }

        public Task<OperationResult<bool>> DeleteTask(int id)
        {
            throw new NotImplementedException();
        }
    }
}
