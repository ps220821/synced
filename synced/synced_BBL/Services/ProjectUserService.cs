using System;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
using synced_DAL.Interfaces;
using synced_DALL.Entities;
using synced_DALL.Interfaces;

namespace synced_BBL.Services
{
    public class ProjectUserService : IProjectUserService
    {
        private readonly IUserProjectRepository _userProjectRepository;
        private readonly IUserRepository _userRepository;

        public ProjectUserService(IUserProjectRepository userProjectRepository, IUserRepository userRepository)
        {
            _userProjectRepository = userProjectRepository;
            _userRepository = userRepository;
        }

        public async Task<OperationResult<bool>> AddUserToProject(int projectId, int userId, string email)
        {
            try
            {
                // Controleer of gebruiker bestaat
                int existingUserId = await _userRepository.CheckEmailExists(email);
                if (existingUserId == 0)
                {
                    return OperationResult<bool>.Failure("User with this email does not exist.");
                }

                // Maak association via rich‐factory
                ProjectUsers projectUser = ProjectUsers.Create(
                    projectId,
                    userId,
                    Roles.member
                );

                int rowsAffected = await _userProjectRepository.AddUserToProject(projectUser);
                return rowsAffected > 0
                    ? OperationResult<bool>.Success(true)
                    : OperationResult<bool>.Failure("Failed to add user to project.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
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
                return rowsAffected > 0
                    ? OperationResult<bool>.Success(true)
                    : OperationResult<bool>.Failure("Failed to remove user from project or user not found.");
            }
            catch (DatabaseException ex)
            {
                return OperationResult<bool>.Failure(ex.Message);
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
                var associations = await _userProjectRepository.GetProjectUsers(projectId);

                var userDtos = associations
                    .Select(pu => new UserDto
                    {
                        Id = pu.UserId,
                        Firstname = pu.User.Firstname,
                        Lastname = pu.User.Lastname,
                        Email = pu.User.Email,
                        Password = pu.User.Password,
                        Created_at = pu.User.CreatedAt
                    })
                    .ToList();

                return OperationResult<List<UserDto>>.Success(userDtos);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<List<UserDto>>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<List<UserDto>>.Failure("An unexpected error occurred while retrieving users.");
            }
        }
    }
}
