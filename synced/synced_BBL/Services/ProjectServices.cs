using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL.Interfaces;
using synced_DALL.Entities;
using synced_DALL.Interfaces;

namespace synced_BBL.Services
{
    public class ProjectServices : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserProjectRepository _userProjectRepository;

        public ProjectServices(
            IProjectRepository projectRepository,
            IUserProjectRepository userProjectRepository)
        {
            _projectRepository = projectRepository;
            _userProjectRepository = userProjectRepository;
        }

        public async Task<OperationResult<List<ProjectDto>>> GetAllProjects(int userId)
        {
            try
            {
                var projects = await _projectRepository.GetAllAsync(userId);
                var projectDtos = projects.Select(project => new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    Start_Date = project.Start_Date,
                    End_Date = project.End_Date,
                    OwnerId = project.OwnerId
                }).ToList();

                return OperationResult<List<ProjectDto>>.Success(projectDtos);
            }
            catch (Exception)
            {
                return OperationResult<List<ProjectDto>>.Failure("Unexpected error while fetching projects.");
            }
        }

        public async Task<OperationResult<bool>> CreateProject(ProjectDto projectDto)
        {
            try
            {
                // Map DTO → rich Project entity via factory
                Project mappedProject = Project.Create(
                    projectDto.Name,
                    projectDto.Description,
                    projectDto.Start_Date,
                    projectDto.End_Date,
                    projectDto.OwnerId
                );

                int newProjectId = await _projectRepository.CreateAsync(mappedProject);

                if (newProjectId <= 0)
                    return OperationResult<bool>.Failure("Project could not be created.");

                ProjectUsers projectUser = ProjectUsers.Create(
                     newProjectId,
                     mappedProject.OwnerId,
                     Roles.admin
                 );

                int rowsAffected = await _userProjectRepository.AddUserToProject(projectUser);

                return rowsAffected > 0
                    ? OperationResult<bool>.Success(true)
                    : OperationResult<bool>.Failure("User could not be added to project.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("Unexpected error while creating project.");
            }
        }

        public async Task<OperationResult<bool>> DeleteProject(int projectId)
        {
            try
            {
                int rowsAffected = await _projectRepository.DeleteAsync(projectId);
                if (rowsAffected == 0)
                    return OperationResult<bool>.Failure("No project found to delete.");

                return OperationResult<bool>.Success(true);
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("Unexpected error while deleting project.");
            }
        }
    }
}
