using Moq;
using synced_BBL.Services;
using synced_BBL.Interfaces;
using synced_DALL.Interfaces;
using synced_DALL.Entities;
using synced_BBL.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;  // Of NUnit, afhankelijk van je keuze
using synced.Core.Results;
using System;
using Task = System.Threading.Tasks.Task;  // Voor DateOnly

namespace synced_tests
{
    public class ProjectTests
    {
        private readonly Mock<IProjectRepository> _projectRepoMock;
        private readonly Mock<IUserProjectRepository> _userProjectRepoMock;
        private readonly ProjectServices _projectService;

        public ProjectTests()
        {
            // Mocks voor de repositories
            _projectRepoMock = new Mock<IProjectRepository>();
            _userProjectRepoMock = new Mock<IUserProjectRepository>();
            _projectService = new ProjectServices(_projectRepoMock.Object, _userProjectRepoMock.Object);
        }

        [Fact]
        public async Task GetAllProjects_ReturnsSuccessResult_WithProjectDtos()
        {
            // Arrange
            int userId = 1;
            var projects = new List<Project>
            {
                Project.FromExisting(
                    1,
                    "Project1",
                    "Desc1",
                    DateOnly.FromDateTime(DateTime.Now),
                    DateOnly.FromDateTime(DateTime.Now),
                    userId
                ),
                Project.FromExisting(
                    2,
                    "Project2",
                    "Desc2",
                    DateOnly.FromDateTime(DateTime.Now),
                    DateOnly.FromDateTime(DateTime.Now),
                    userId
                )
            };

            _projectRepoMock.Setup(repo => repo.GetAllAsync(userId)).ReturnsAsync(projects);

            // Act
            var result = await _projectService.GetAllProjects(userId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("Project1", result.Data[0].Name);
            Assert.Equal("Project2", result.Data[1].Name);
        }

        [Fact]
        public async Task CreateProject_ReturnsSuccess_WhenProjectIsCreatedAndUserLinked()
        {
            // Arrange
            var projectDto = new ProjectDto
            {
                Name = "New Project",
                Description = "Beschrijving",
                Start_Date = DateOnly.FromDateTime(DateTime.Today),
                End_Date = DateOnly.FromDateTime(DateTime.Today.AddDays(30)),
                Owner = 1
            };

            var mappedProject = Project.Create(
                projectDto.Name,
                projectDto.Description,
                projectDto.Start_Date,
                projectDto.End_Date,
                projectDto.Owner
            );

            int fakeProjectId = 42;

            // Setup de repo-mocks
            _projectRepoMock.Setup(repo => repo.CreateAsync(It.IsAny<Project>())).ReturnsAsync(fakeProjectId);
            _userProjectRepoMock.Setup(repo => repo.AddUserToProject(It.IsAny<ProjectUsers>())).ReturnsAsync(1);

            // Act
            var result = await _projectService.CreateProject(projectDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task CreateProject_ReturnsFailure_WhenInsertFails()
        {
            // Arrange
            var projectDto = new ProjectDto
            {
                Name = "Test Project",
                Description = "Test Beschrijving",
                Start_Date = DateOnly.FromDateTime(DateTime.Today),
                End_Date = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
                Owner = 1
            };

            _projectRepoMock.Setup(x => x.CreateAsync(It.IsAny<Project>())).ReturnsAsync(0); // Simuleer mislukking

            // Act
            var result = await _projectService.CreateProject(projectDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("Project creation failed.", result.Message);
        }

        [Fact]
        public async Task DeleteProject_ReturnsSuccess_WhenRowsAffectedGreaterThanZero()
        {
            // Arrange
            int projectId = 1;
            // Simuleer dat er 1 rij is verwijderd
            _projectRepoMock.Setup(repo => repo.DeleteAsync(projectId)).ReturnsAsync(1);

            // Act
            var result = await _projectService.DeleteProject(projectId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data);  // True als verwijdering succesvol is
        }

        [Fact]
        public async Task DeleteProject_ReturnsFailure_WhenNoProjectFound()
        {
            // Arrange
            int projectId = 1;
            // Simuleer dat geen rijen zijn verwijderd (project bestaat niet)
            _projectRepoMock.Setup(repo => repo.DeleteAsync(projectId)).ReturnsAsync(0);

            // Act
            var result = await _projectService.DeleteProject(projectId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("No project found to delete.", result.Message);
        }
    }
}
