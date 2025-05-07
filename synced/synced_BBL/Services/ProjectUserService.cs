using Microsoft.Data.SqlClient;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_DAL;
using synced_DALL.Entities;
using synced_DAL.Interfaces;
using synced_DALL.Interfaces;
using synced_DALL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using synced_BBL.Interfaces;

namespace synced_BBL.Services
{
    public class ProjectUserService : IProjectUserService
    {
        private readonly IUserProjectRepository _userProjectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectUserService( IUserProjectRepository userProjectRepository, IUserRepository userRepository)
        {
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;
        }

        public async Task<OperationResult<bool>> AddUserToProject(int projectId, int userId, string email)
        {
            try
            {
                var user = await _userRepository.CheckEmailExists(email);  // Dit kan je aanpassen afhankelijk van hoe je gebruikers ophaalt

                if (user == null)
                {
                    return OperationResult<bool>.Failure("User with this email does not exist.");
                }

                ProjectUsers projectUser = new ProjectUsers
                {
                    ProjectId = projectId,
                    UserId = userId,
                    Role = Roles.member
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
                List<ProjectUsers> users = await _userProjectRepository.GetProjectUsers(projectId);

                var userDtos = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    Firstname = user.User.Firstname,
                    Lastname = user.User.Lastname,
                    Email = user.User.Email,
                    Password = user.User.Password, // Opmerking: wachtwoord retourneren is meestal niet veilig!
                    Created_at = user.User.CreatedAt
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
