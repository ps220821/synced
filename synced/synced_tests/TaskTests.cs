using Moq;
using synced_BBL.Services;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using synced_BBL.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Task = synced_DALL.Entities.Task;

namespace synced_tests
{
    public class TaskTests
    {
        private readonly Mock<ITaskRepository> _taskRepoMock;
        private readonly TaskService _taskService;

        public TaskTests()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _taskService = new TaskService(_taskRepoMock.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllTasks_ReturnsSuccessWithGroupedTasks_WhenTasksExist()
        {
            // Arrange
            var tasks = new List<Task>
            {
                Task.FromExisting(
                    1,
                    "Task 1",
                    "",
                    Status.todo,
                    Priorities.medium,
                    DateTime.Now,
                    null,
                    1
                ),
                Task.FromExisting(
                    2,
                    "Task 2",
                    "",
                    Status.inprogress,
                    Priorities.high,
                    DateTime.Now.AddDays(1),
                    null,
                    1
                ),
                Task.FromExisting(
                    3,
                    "Task 3",
                    "",
                    Status.done,
                    Priorities.low,
                    DateTime.Now.AddDays(-1),
                    null,
                    1
                )
            };

            // Setup mock to return the tasks
            _taskRepoMock.Setup(x => x.GetAllAsync(1)).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasks(1);

            // Assert
            Assert.True(result.Succeeded);  // Ensure the operation is successful
            Assert.NotNull(result.Data);    // Ensure data is returned
            Assert.Single(result.Data.TodoTasks);       // Check if there is 0 task in "todo"
            Assert.Single(result.Data.InProgressTasks); // Check if there is 1 task in "inprogress"
            Assert.Single(result.Data.DoneTasks);       // Check if there is 2 task in "done"
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllTasks_ReturnsFailure_WhenNoTasksExist()
        {
            // Arrange: Simulating no tasks in the database
            _taskRepoMock.Setup(x => x.GetAllAsync(1)).ReturnsAsync(new List<Task>());

            // Act
            var result = await _taskService.GetAllTasks(1);

            // Assert
            Assert.False(result.Succeeded);  // Operation should fail when no tasks exist
            Assert.Equal("An unexpected error occurred while fetching tasks.", result.Message);  // Check error message
        }


        [Fact]
        public async System.Threading.Tasks.Task CreateTask_ReturnsSuccess_WhenTaskIsCreated()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Title = "New Task",
                Description = "Task description",
                Status = Status.todo,
                Priority = Priorities.medium,
                Deadline = DateTime.Now.AddDays(5),
                ProjectId = 1,
                UserId = 1
            };

            var newTask = Task.FromExisting(
                1,
                taskDto.Title,
                taskDto.Description,
                taskDto.Status,
                taskDto.Priority,
                taskDto.Deadline,
                taskDto.UserId,
                taskDto.ProjectId
            );

            _taskRepoMock.Setup(x => x.CreateAsync(It.IsAny<Task>())).ReturnsAsync(1); 

            // Act
            var result = await _taskService.CreateTask(taskDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data);  
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTask_ReturnsFailure_WhenTaskCreationFails()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Title = "New Task",
                Description = "Task description",
                Status = Status.todo,
                Priority = Priorities.medium,
                Deadline = DateTime.Now.AddDays(5),
                ProjectId = 1,
                UserId = 1
            };

            _taskRepoMock.Setup(x => x.CreateAsync(It.IsAny<Task>())).ReturnsAsync(0);  // Simuleer mislukking

            // Act
            var result = await _taskService.CreateTask(taskDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Task could not be created.", result.Message);
        }


        // update 
        [Fact]
        public async System.Threading.Tasks.Task UpdateTask_ReturnsSuccess_WhenTaskIsUpdated()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Id = 1,
                Title = "Updated Task",
                Description = "Updated description",
                Status = Status.inprogress,
                Priority = Priorities.high,
                Deadline = DateTime.Now.AddDays(2),
                ProjectId = 1,
                UserId = 1
            };

            var updatedTask = Task.FromExisting(
                taskDto.Id,
                taskDto.Title,
                taskDto.Description,
                taskDto.Status,
                taskDto.Priority,
                taskDto.Deadline,
                taskDto.UserId,
                taskDto.ProjectId
            );

            _taskRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(1);  // Simuleer succes

            // Act
            var result = await _taskService.UpdateTask(taskDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data);  // Aangeven dat de taak succesvol is geüpdatet
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTask_ReturnsFailure_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskDto = new TaskDto
            {
                Id = 1,
                Title = "Updated Task",
                Description = "Updated description",
                Status = Status.inprogress,
                Priority = Priorities.high,
                Deadline = DateTime.Now.AddDays(2),
                ProjectId = 1,
                UserId = 1
            };

            _taskRepoMock.Setup(x => x.UpdateAsync(It.IsAny<Task>())).ReturnsAsync(0);  // Simuleer mislukking

            // Act
            var result = await _taskService.UpdateTask(taskDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Task could not be updated or does not exist.", result.Message);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTask_ReturnsSuccess_WhenTaskIsDeleted()
        {
            int taskId = 1;
            _taskRepoMock.Setup(x => x.DeleteAsync(taskId)).ReturnsAsync(true);

            var result = await _taskService.DeleteTask(taskId);

            Assert.True(result.Succeeded);
            Assert.True(result.Data);
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTask_ReturnsFailure_WhenTaskNotFound()
        {
            int taskId = 1;
            _taskRepoMock.Setup(x => x.DeleteAsync(taskId)).ReturnsAsync(false);

            var result = await _taskService.DeleteTask(taskId);

            Assert.False(result.Succeeded);
            Assert.Equal("Task could not be deleted or does not exist.", result.Message);
        }

    }
}
