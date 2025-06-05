using Moq;
using synced_BBL.Services;
using synced_DAL.Interfaces;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
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
                Id = userId,
                Username = "user",
                Firstname = "First",
                Lastname = "User",
                Email = email,
                CreatedAt = DateTime.Now
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
                Id = userId,
                Username = "user",
                Firstname = "First",
                Lastname = "User",
                Email = email,
                CreatedAt = DateTime.Now
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
            var projectUsers = new List<ProjectUsers>
    {
        new ProjectUsers
        {
            Id = 1,
            ProjectId = projectId,
            Project = new Project { Id = projectId, Name = "Project 1" }, // Stel een project in (optioneel)
            UserId = 1,
            User = new User
            {
                Id = 1,
                Username = "user1",
                Firstname = "First",
                Lastname = "User",
                Email = "user1@example.com",
                Password = "password123",
                CreatedAt = DateTime.Now
            },
            Role = Roles.member
        },
        new ProjectUsers
            {
                Id = 2,
                ProjectId = projectId,
                Project = new Project { Id = projectId, Name = "Project 1" }, // Stel een project in (optioneel)
                UserId = 2,
                User = new User
                {
                    Id = 2,
                    Username = "user2",
                    Firstname = "Second",
                    Lastname = "User",
                    Email = "user2@example.com",
                    Password = "password123",
                    CreatedAt = DateTime.Now
                },
                Role = Roles.admin
                }
            };

            // Stel de mock in om de lijst van ProjectUsers terug te geven
            _userProjectRepoMock
                .Setup(repo => repo.GetProjectUsers(projectId))
                .ReturnsAsync(projectUsers);

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
            var users = new List<ProjectUsers>();  

            _userProjectRepoMock.Setup(repo => repo.GetProjectUsers(projectId)).ReturnsAsync(users);

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

            _userProjectRepoMock.Setup(repo => repo.GetProjectUsers(projectId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _projectUserService.GetAllUsers(projectId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("An unexpected error occurred while retrieving users.", result.Message);
        }
    }
}
