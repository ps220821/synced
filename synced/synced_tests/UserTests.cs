using Moq;
using System.Threading.Tasks;
using Xunit;
using synced_BBL.Services;
using synced_DAL.Interfaces;
using synced_BBL.Dtos;
using synced.Core.Results;
using System;
using Task = System.Threading.Tasks.Task;


namespace synced_tests
{
    public class UserTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly UserService _userService;

        public UserTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepoMock.Object);
        }

        [Fact]
        public async Task GetUserByEmail_ReturnsUserId()
        {
            // Arrange
            string email = "test@example.com";
            _userRepoMock.Setup(x => x.CheckEmailExists(email)).ReturnsAsync(5);

            // Act
            var result = await _userService.GetUserByEmail(email);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task LoginUser_ReturnsSuccess_WhenCredentialsAreCorrect()
        {
            // Arrange
            var loginDto = new LoginDto { email = "test@example.com", password = "1234" };
            _userRepoMock.Setup(x => x.Login(loginDto.email, loginDto.password)).ReturnsAsync(10);

            // Act
            var result = await _userService.LoginUser(loginDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(10, result.Data);
        }

        [Fact]
        public async Task LoginUser_ReturnsFailure_WhenCredentialsAreIncorrect()
        {
            // Arrange
            var loginDto = new LoginDto { email = "wrong@example.com", password = "wrongpass" };
            _userRepoMock.Setup(x => x.Login(loginDto.email, loginDto.password)).ReturnsAsync(0); // Geen gebruiker gevonden

            // Act
            var result = await _userService.LoginUser(loginDto);

            // Assert
            Assert.False(result.Succeeded); // De login moet mislukken
            Assert.Equal("Invalid email or password.", result.Message); // Verwacht bericht
        }

        [Fact]
        public async Task RegisterUser_ReturnsFailure_WhenEmailAlreadyExists()
        {
            // Arrange
            var userDto = new UserDto { Email = "bestaand@example.com", Password = "1234" };
            _userRepoMock.Setup(x => x.CheckEmailExists(userDto.Email)).ReturnsAsync(1); // E-mail bestaat al

            // Act
            var result = await _userService.RegisterUser(userDto);

            // Assert
            Assert.False(result.Succeeded); // De registratie moet mislukken
            Assert.Equal("Email already exists.", result.Message); // Verwacht bericht
        }
    }
}
