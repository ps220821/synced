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

        public int GetUserBYEmail(string email)
        {
            return this._userRepository.GetUserByEmail(email);
        }

        public async Task<OperationResult<int>> LoginUser(LoginDto login)
        {
            try
            {
                if (string.IsNullOrEmpty(login.email) || string.IsNullOrEmpty(login.password))
                {
                    return OperationResult<int>.Failure("Email and password must not be empty.");
                }

                var result = _userRepository.Login(login.email, login.password);

                if (result.Succeeded)
                {
                    return OperationResult<int>.Success(result.Data);
                }

                return result;
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
                var mappedUser = Mapper.MapCreate<User>(userDto);

                // Check if the mappedUser is null
                if (mappedUser == null)
                {
                    return OperationResult<int>.Failure("User mapping failed.");
                }
                var result = _userRepository.Register(mappedUser);

                if (result.Succeeded)
                {
                    return OperationResult<int>.Success(result.Data);
                }

                return result;
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
