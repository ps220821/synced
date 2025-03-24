using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL.Entities;
using synced_DALL.Interfaces;
using synced_DALL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Services
{
    public class ProjectUserService : IProjectUserService
    {
        private readonly IUserProjectRepository _userProjectRepository;

        public ProjectUserService(IProjectRepository projectRepository, IUserProjectRepository userProjectRepository)
        {
            _userProjectRepository = userProjectRepository;
        }

        public bool AddUserToProject(int projectId, int userId)
        {
            Project_users project_Users = new Project_users
            {
                project_id = projectId,
                user_id = userId,
                roles = Roles.member
            }; 

            return this._userProjectRepository.AddUserToProject(project_Users);
        }

        public bool RemoveUserFromProject(int userId, int projectId)
       {
            return this._userProjectRepository.RemoveUserFromProject(userId,projectId);
       }

        public List<UserDto> GetAllUsers(int projectId)
        {
            List<User> users = this._userProjectRepository.GetAllUsers(projectId);


            List<UserDto> userList = users.Select(user => new UserDto
            {
                Id = user.id,
                Firstname = user.firstname,
                Lastname = user.lastname,
                Email = user.email,
                Password = user.password,
                Created_at = user.created_at,
            }).ToList();
            return userList;
        }
    }
}
