using Microsoft.Data.SqlClient;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
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
                Project_users projectUser = new Project_users
                {
                    project_id = projectId,
                    user_id = userId,
                    roles = Roles.member
                };

                int rowsAffected = await _userProjectRepository.AddUserToProject(projectUser);

                if (rowsAffected > 0)
                {
                    return OperationResult<bool>.Success(true);
                }

                return OperationResult<bool>.Failure("Failed to add user to project.");
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

        public async Task<OperationResult<bool>> RemoveUserFromProject(int userId, int projectId)
        {
            try
            {
                int rowsAffected = await _userProjectRepository.RemoveUserFromProject(userId, projectId);

                if (rowsAffected > 0)
                {
                    return OperationResult<bool>.Success(true);
                }

                return OperationResult<bool>.Failure("Failed to remove user from project or user not found.");
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
                return OperationResult<bool>.Failure("An unexpected error occurred while removing user from project.");
            }

        }

        public async Task<OperationResult<List<UserDto>>> GetAllUsers(int projectId)
        {
            try
            {
                var users = await _userProjectRepository.GetAllUsers(projectId);

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.id,
                    Firstname = user.firstname,
                    Lastname = user.lastname,
                    Email = user.email,
                    Password = user.password, // Opmerking: wachtwoord retourneren is meestal niet veilig!
                    Created_at = user.created_at
                }).ToList();

                return OperationResult<List<UserDto>>.Success(userDtos);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<List<UserDto>>.Failure(ex.Message);
            }
            catch (SqlException ex)
            {
                return OperationResult<List<UserDto>>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<List<UserDto>>.Failure("An unexpected error occurred while retrieving users.");
            }
        }
    }
}
