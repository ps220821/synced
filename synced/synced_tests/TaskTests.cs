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
                new Task
                {
                    Id = 1,
                    Title = "Task 1",
                    Status = Status.todo,
                    Priority = Priorities.medium,
                    Deadline = DateTime.Now,
                    ProjectId = 1
                },
                new Task
                {
                    Id = 2,
                    Title = "Task 2",
                    Status = Status.inprogress,
                    Priority = Priorities.high,
                    Deadline = DateTime.Now.AddDays(1),
                    ProjectId = 1
                },
                new Task
                {
                    Id = 3,
                    Title = "Task 3",
                    Status = Status.done,
                    Priority = Priorities.low,
                    Deadline = DateTime.Now.AddDays(-1),
                    ProjectId = 1
                }
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

            var newTask = new Task
            {
                Id = 1,
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                Deadline = taskDto.Deadline,
                ProjectId = taskDto.ProjectId,
                UserId = taskDto.UserId
            };

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

            var updatedTask = new Task
            {
                Id = taskDto.Id,
                Title = taskDto.Title,
                Description = taskDto.Description,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                Deadline = taskDto.Deadline,
                ProjectId = taskDto.ProjectId,
                UserId = taskDto.UserId,
            };

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

    }
}
