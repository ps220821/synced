using synced.Core.Results;
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

        public async Task<OperationResult<bool>> AddUserToProject(int projectId, int userId)
        {
            try
            {
                Project_users project_Users = new Project_users
                {
                    project_id = projectId,
                    user_id = userId,
                    roles = Roles.member
                };
                var success =  _userProjectRepository.AddUserToProject(project_Users);

                return OperationResult<bool>.Success(success);
            }
            catch (Exception ex)
            {
                return OperationResult<bool>.Failure("Failed to add user to project.");
            }


        }

        public async Task<OperationResult<bool>> RemoveUserFromProject(int userId, int projectId)
           {
                try
                {
                    var result = _userProjectRepository.RemoveUserFromProject(userId, projectId);
                    return OperationResult<bool>.Success(result);
                }
                catch (Exception ex)
                {
                    return OperationResult<bool>.Failure("Failed to remove user from project.");
                }

            }

        public async Task<OperationResult<List<UserDto>>> GetAllUsers(int projectId)
        {
            try
            {
                var users = _userProjectRepository.GetAllUsers(projectId);

                if (users == null)
                {
                    return OperationResult<List<UserDto>>.Failure("No users found.");
                }

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.id,
                    Firstname = user.firstname,
                    Lastname = user.lastname,
                    Email = user.email,
                    Password = user.password,
                    Created_at = user.created_at,
                }).ToList();

                return OperationResult<List<UserDto>>.Success(userDtos);
            }
            catch (Exception ex)
            {
                return OperationResult<List<UserDto>>.Failure("Error occurred while retrieving users.");
            }
        }
    }
}
