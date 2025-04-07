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
using System.Threading.Tasks;
using velocitaApi.Mappers;

namespace synced_BBL.Services
{
    public class ProjectServices : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUserProjectRepository _userProjectRepository;

        public ProjectServices(IProjectRepository projectRepository, IUserProjectRepository userProjectRepository)
        {
            _projectRepository = projectRepository;
            _userProjectRepository = userProjectRepository;
        }

        public async Task<OperationResult<List<ProjectDto>>> GetAllProjects(int id)
        {
            try
            {
                var projects = _projectRepository.GetAllAsync(id);

                var projectDtos = projects.Select(project => new ProjectDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    Start_Date = project.Start_Date,
                    End_Date = project.End_Date,
                    Owner = project.Owner
                }).ToList();

                return OperationResult<List<ProjectDto>>.Success(projectDtos);
            }
            catch (SqlException ex)
            {
                return OperationResult<List<ProjectDto>>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<List<ProjectDto>>.Failure("Unexpected error while fetching projects.");
            }
        }

        public async Task<OperationResult<bool>> CreateProject(ProjectDto project)
        {
            try
            {
                var mappedProject = Mapper.MapCreate<Project>(project);
                if (mappedProject == null)
                    return OperationResult<bool>.Failure("Project mapping failed.");

                int newProjectId = _projectRepository.CreateAsync(mappedProject);

                if (newProjectId <= 0)
                    return OperationResult<bool>.Failure("Project could not be created.");

                var projectUser = new Project_users
                {
                    user_id = mappedProject.Owner,
                    project_id = newProjectId,
                    roles = Roles.admin // Zorg dat Roles een enum of constant is
                };

                int rowsAffected = await _userProjectRepository.AddUserToProject(projectUser);

                return rowsAffected > 0
                    ? OperationResult<bool>.Success(true)
                    : OperationResult<bool>.Failure("User could not be added to project.");
            }
            catch (SqlException ex)
            {
                return OperationResult<bool>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("Unexpected error while creating project.");
            }
        }

        public async Task<OperationResult<bool>> DeleteProject(int id)
        {
            try
            {
                int rowsAffected = _projectRepository.DeleteAsync(id);

                if (rowsAffected == 0)
                    return OperationResult<bool>.Failure("No project found to delete.");

                return OperationResult<bool>.Success(true);
            }
            catch (SqlException ex)
            {
                return OperationResult<bool>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<bool>.Failure("Unexpected error while deleting project.");
            }
        }
    }
}
