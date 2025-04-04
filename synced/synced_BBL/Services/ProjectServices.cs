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
            catch (DatabaseException ex)
            {
                return OperationResult<List<ProjectDto>>.Failure(ex.Message);
            }
            catch (SqlException ex)
            {
                return OperationResult<List<ProjectDto>>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<List<ProjectDto>>.Failure("An unexpected error occurred while adding user to project.");
            }
        }

        public async Task<OperationResult<bool>> CreateProject(ProjectDto project)
        {
            try
            {
                var mappedProject = Mapper.MapCreate<Project>(project);

                if (mappedProject == null)
                {
                    return OperationResult<bool>.Failure("Mapping failed.");
                }

                int newProject = _projectRepository.CreateAsync(mappedProject);

                if (newProject > 0)
                {
                    Project_users projectUser = new Project_users
                    {
                        user_id = mappedProject.Owner,
                        project_id = newProject,
                        roles = Roles.admin,
                    };

                    bool added = _userProjectRepository.AddUserToProject(projectUser);
                    return OperationResult<bool>.Success(added);
                }

                return OperationResult<bool>.Failure("Project could not be created.");
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
                return OperationResult<bool>.Failure("An unexpected error occurred while adding user to project.");
            }
        }

        public async Task<OperationResult<bool>> DeleteProject(int id)
        {
            try
            {
                bool deleted = _projectRepository.DeleteAsync(id);

                if (deleted)
                {
                    return OperationResult<bool>.Success(true);
                }

                return OperationResult<bool>.Failure("Project could not be deleted.");
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
                return OperationResult<bool>.Failure("An unexpected error occurred while adding user to project.");
            }
        }
    }
}
