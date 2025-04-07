using Microsoft.Data.SqlClient;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
using synced_DAL.Entities;
using synced_DAL.Interfaces;
using synced_DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using velocitaApi.Mappers;

namespace synced_BBL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> GetUserByEmail(string email)
        {
            return await _userRepository.CheckEmailExists(email);
        }

        public async Task<OperationResult<int>> LoginUser(LoginDto login)
        {
            try
            {
                if (string.IsNullOrEmpty(login.email) || string.IsNullOrEmpty(login.password))
                {
                    return OperationResult<int>.Failure("Email and password must not be empty.");
                }

                int userId = await _userRepository.Login(login.email, login.password);

                if (userId > 0)
                {
                    return OperationResult<int>.Success(userId);
                }

                return OperationResult<int>.Failure("Invalid email or password.");
            }
            catch (DatabaseException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (SqlException ex) 
            {
                return OperationResult<int>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Failure("An unexpected error occurred. Please try again.");
            }
        }

        public async Task<OperationResult<int>> RegisterUser(UserDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return OperationResult<int>.Failure("User data cannot be null.");
                }

                if (string.IsNullOrEmpty(userDto.Email))
                {
                    return OperationResult<int>.Failure("Email cannot be empty.");
                }

                if (string.IsNullOrEmpty(userDto.Password))
                {
                    return OperationResult<int>.Failure("Password cannot be empty.");
                }

                int existingUserId = await GetUserByEmail(userDto.Email);
                if (existingUserId > 0)
                {
                    return OperationResult<int>.Failure("Email already exists.");
                }

                var mappedUser = Mapper.MapCreate<User>(userDto);
                if (mappedUser == null)
                {
                    return OperationResult<int>.Failure("User mapping failed.");
                }

                int newUserId = await _userRepository.Register(mappedUser);

                if (newUserId > 0)
                {
                    return OperationResult<int>.Success(newUserId);
                }

                return OperationResult<int>.Failure("Failed to register user.");
            }
            catch (DatabaseException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (SqlException ex) 
            {
                return OperationResult<int>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception ex)
            {
                return OperationResult<int>.Failure("An unexpected error occurred. Please try again.");
            }
            
        }
    }
}
