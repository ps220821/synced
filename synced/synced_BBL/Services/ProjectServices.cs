using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL.Entities;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<ProjectDto> GetAllProjects(int id)
        {
            var projects = this._projectRepository.GetAllAsync(id);

            List<ProjectDto> projectDtos = projects.Select(project => new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Start_Date = project.Start_Date,
                End_Date = project.End_Date,
                Owner = project.Owner
            }).ToList();


            return projectDtos;
        }

        public bool CreateProject(ProjectDto project)
        {
            var mappedUser = Mapper.MapCreate<Project>(project);

            int newProject = _projectRepository.CreateAsync(mappedUser); // Call only once

            if (newProject > 0)  // Use the stored result instead of calling again
            {
                Project_users projectUser = new Project_users
                {
                    user_id = mappedUser.Owner,
                    project_id = newProject,
                    roles = Roles.admin,
                };
                return _userProjectRepository.AddUserToProject(projectUser);
            }
            return false;
        }

        public bool DeleteProject(int id)
        {
            return _projectRepository.DeleteAsync(id);
        }

    }
}
