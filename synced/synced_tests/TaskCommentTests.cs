using Moq;
using synced_BBL.Services;
using synced_BBL.Interfaces;
using synced_BBL.Dtos;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Task = System.Threading.Tasks.Task;
using synced_DALL.Entities;

namespace synced_tests
{
    public class TaskCommentTests
    {
        private readonly Mock<ItaskCommentRepository> _taskCommentRepoMock;
        private readonly TaskCommentService _taskCommentService;

        public TaskCommentTests()
        {
            _taskCommentRepoMock = new Mock<ItaskCommentRepository>();
            _taskCommentService = new TaskCommentService(_taskCommentRepoMock.Object);
        }

        [Fact]
        public async Task AddComment_ReturnsSuccess_WhenCommentIsAdded()
        {
            // Arrange
            var taskCommentDto = new TaskCommentDto
            {
                UserId = 1,
                TaskId = 1,
                Comment = "This is a test comment.",
                CreatedAt = DateTime.Now
            };

            // Simuleer een succesvolle creatie van een comment (id > 0)
            _taskCommentRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<TaskComment>())).ReturnsAsync(1);

            // Act
            var result = await _taskCommentService.AddComment(taskCommentDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.Data); // De nieuwe comment ID
        }

        [Fact]
        public async Task AddComment_ReturnsFailure_WhenCommentIsNull()
        {
            // Arrange
            TaskCommentDto taskCommentDto = null;

            // Act
            var result = await _taskCommentService.AddComment(taskCommentDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Comment data cannot be null.", result.Message);
        }

        [Fact]
        public async Task AddComment_ReturnsFailure_WhenCommentIsEmpty()
        {
            // Arrange
            var taskCommentDto = new TaskCommentDto
            {
                UserId = 1,
                TaskId = 1,
                Comment = "",
                CreatedAt = DateTime.Now
            };

            // Act
            var result = await _taskCommentService.AddComment(taskCommentDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Comment cannot be empty.", result.Message);
        }

        [Fact]
        public async Task AddComment_ReturnsFailure_WhenMappingFails()
        {
            // Arrange
            var taskCommentDto = new TaskCommentDto
            {
                UserId = 1,
                TaskId = 1,
                Comment = "Valid comment",
                CreatedAt = DateTime.Now
            };

            // Simuleer een mappingfout door een null waarde terug te geven
            _taskCommentRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<TaskComment>())).ReturnsAsync(0);

            // Act
            var result = await _taskCommentService.AddComment(taskCommentDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Failed to add comment.", result.Message);
        }

        [Fact]
        public async Task GetTaskComments_ReturnsSuccess_WhenCommentsAreFound()
        {
            // Arrange
            int taskId = 1;
            var taskComments = new List<TaskComment>
            {
                new TaskComment
                {
                    Id = 1,
                    UserId = 1,
                    TaskId = taskId,
                    Comment = "First comment",
                    CreatedAt = DateTime.Now,
                },
                new TaskComment
                {
                    Id = 2,
                    UserId = 2,
                    TaskId = taskId,
                    Comment = "Second comment",
                    CreatedAt = DateTime.Now,
                }
            };

            _taskCommentRepoMock.Setup(repo => repo.GetAllAsync(taskId)).ReturnsAsync(taskComments);

            // Act
            var result = await _taskCommentService.GetTaskComments(taskId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("First comment", result.Data[0].Comment);
            Assert.Equal("Second comment", result.Data[1].Comment);
        }

        [Fact]
        public async Task GetTaskComments_ReturnsFailure_WhenNoCommentsFound()
        {
            // Arrange
            int taskId = 1;
            var taskComments = new List<TaskComment>(); // Geen opmerkingen gevonden

            _taskCommentRepoMock.Setup(repo => repo.GetAllAsync(taskId)).ReturnsAsync(taskComments);

            // Act
            var result = await _taskCommentService.GetTaskComments(taskId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An unexpected error occurred while fetching comments.", result.Message);
        }

        [Fact]
        public async Task GetTaskComments_ReturnsFailure_WhenExceptionOccurs()
        {
            // Arrange
            int taskId = 1;

            // Simuleer een exception (bijv. DatabaseException) bij het ophalen van opmerkingen
            _taskCommentRepoMock.Setup(repo => repo.GetAllAsync(taskId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _taskCommentService.GetTaskComments(taskId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An unexpected error occurred while fetching comments.", result.Message);
        }
    }
}