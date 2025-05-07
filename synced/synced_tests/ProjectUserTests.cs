using Moq;
using synced_BBL.Services;
using synced_DAL.Entities;
using synced_DAL.Interfaces;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace synced_tests
{
    public class ProjectUserTests
    {
        private readonly Mock<IUserProjectRepository> _userProjectRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock; // Voeg mock voor IUserRepository toe
        private readonly ProjectUserService _projectUserService;

        public ProjectUserTests()
        {
            _userProjectRepoMock = new Mock<IUserProjectRepository>();
            _userRepoMock = new Mock<IUserRepository>();  // Mock voor UserRepository
            _projectUserService = new ProjectUserService(_userProjectRepoMock.Object, _userRepoMock.Object);
        }

        [Fact]
        public async Task AddUserToProject_ReturnsSuccess_WhenUserIsAdded()
        {
            // Arrange
            int projectId = 1;
            int userId = 1;
            string email = "user@example.com";  // Geldig emailadres
            var user = new User
            {
                id = userId,
                username = "user",
                firstname = "First",
                lastname = "User",
                email = email,
                created_at = DateTime.Now
            };

            // Simuleer dat de gebruiker met het gegeven emailadres bestaat
            _userRepoMock.Setup(repo => repo.CheckEmailExists(email)).ReturnsAsync(1);

            // Simuleer dat het toevoegen van de gebruiker succesvol is (meer dan 0 rijen beïnvloed)
            _userProjectRepoMock.Setup(repo => repo.AddUserToProject(It.IsAny<ProjectUsers>())).ReturnsAsync(1);

            // Act
            var result = await _projectUserService.AddUserToProject(projectId, userId, email);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data);  // Succes betekent dat de gebruiker is toegevoegd
        }

        [Fact]
        public async Task AddUserToProject_ReturnsFailure_WhenAddingUserFails()
        {
            // Arrange
            int projectId = 1;
            int userId = 1;
            string email = "user@example.com";  // Geldig emailadres
            var user = new User
            {
                id = userId,
                username = "user",
                firstname = "First",
                lastname = "User",
                email = email,
                created_at = DateTime.Now
            };

            _userRepoMock.Setup(repo => repo.CheckEmailExists(email)).ReturnsAsync(0);

            _userProjectRepoMock.Setup(repo => repo.AddUserToProject(It.IsAny<ProjectUsers>())).ReturnsAsync(0);

            // Act
            var result = await _projectUserService.AddUserToProject(projectId, userId, email);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Failed to add user to project.", result.Message);
        }

        [Fact]
        public async Task AddUserToProject_ReturnsFailure_WhenUserDoesNotExist()
        {
            // Arrange
            int projectId = 1;
            int userId = 1;
            string email = "nonexistentuser@example.com";

            _userRepoMock.Setup(repo => repo.CheckEmailExists(email)).ReturnsAsync(0);

            var result = await _projectUserService.AddUserToProject(projectId, userId, email);

            Assert.False(result.Succeeded);
            Assert.Equal("User with this email does not exist.", result.Message);
        }

        [Fact]
        public async Task RemoveUserFromProject_ReturnsSuccess_WhenUserIsRemoved()
        {
            int projectId = 1;
            int userId = 1;

            _userProjectRepoMock.Setup(repo => repo.RemoveUserFromProject(userId, projectId)).ReturnsAsync(1);

            // Act
            var result = await _projectUserService.RemoveUserFromProject(userId, projectId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data); 
        }

        [Fact]
        public async Task RemoveUserFromProject_ReturnsFailure_WhenUserNotFound()
        {
            int projectId = 1;
            int userId = 1;

            _userProjectRepoMock.Setup(repo => repo.RemoveUserFromProject(userId, projectId)).ReturnsAsync(0);

            var result = await _projectUserService.RemoveUserFromProject(userId, projectId);

            Assert.False(result.Succeeded);
            Assert.Equal("Failed to remove user from project or user not found.", result.Message);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsSuccess_WhenUsersAreFound()
        {
            // Arrange
            int projectId = 1;
            var users = new List<User>
            {
                new User
                {
                    id = 1,
                    username = "user1",
                    firstname = "First",
                    lastname = "User",
                    email = "user1@example.com",
                    password = "password123",
                    created_at = DateTime.Now
                },
                new User
                {
                    id = 2,
                    username = "user2",
                    firstname = "Second",
                    lastname = "User",
                    email = "user2@example.com",
                    password = "password123",
                    created_at = DateTime.Now
                }
            };

            _userProjectRepoMock.Setup(repo => repo.GetAllUsers(projectId)).ReturnsAsync(users);

            // Act
            var result = await _projectUserService.GetAllUsers(projectId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("user1", result.Data[0].Username);
            Assert.Equal("user2", result.Data[1].Username);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsFailure_WhenNoUsersFound()
        {
            // Arrange
            int projectId = 1;
            var users = new List<User>();  

            _userProjectRepoMock.Setup(repo => repo.GetAllUsers(projectId)).ReturnsAsync(users);

            // Act
            var result = await _projectUserService.GetAllUsers(projectId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An unexpected error occurred while retrieving users.", result.Message);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsFailure_WhenExceptionOccurs()
        {
            // Arrange
            int projectId = 1;

            _userProjectRepoMock.Setup(repo => repo.GetAllUsers(projectId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _projectUserService.GetAllUsers(projectId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An unexpected error occurred while retrieving users.", result.Message);
        }
    }
}
