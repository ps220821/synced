using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
using synced_DAL.Interfaces;
using synced_DALL.Entities;
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
                    return OperationResult<int>.Failure("Email and password must not be empty.");

                User user = await _userRepository.GetByMail(login.email);
                if (user == null)
                    return OperationResult<int>.Failure("Invalid email or password.");

                if (!user.VerifyPassword(login.password))
                    return OperationResult<int>.Failure("Invalid email or password.");

                return OperationResult<int>.Success(user.Id);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (Exception)
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

                var existing = await _userRepository.GetByMail(userDto.Email);
                if (existing != null)
                {
                    return OperationResult<int>.Failure("Email already exists.");
                }

                User newUser = User.Create(
                    userDto.Username,
                    userDto.Firstname,
                    userDto.Lastname,
                    userDto.Email,
                    userDto.Password
                );

                int newUserId = await _userRepository.Register(newUser);

                if (newUserId > 0)
                {
                    return OperationResult<int>.Success(newUserId);
                }

                return OperationResult<int>.Failure("Failed to register user.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<int>.Failure("An unexpected error occurred. Please try again.");
            }
        }
    }
}