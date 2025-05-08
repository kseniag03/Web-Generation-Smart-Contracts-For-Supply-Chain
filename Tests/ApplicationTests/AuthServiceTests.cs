using Application.Common;
using Application.Interfaces;
using Application.Services;
using Moq;

namespace Tests.ApplicationTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _authService = new AuthService(_authRepositoryMock.Object);
        }

        [Fact]
        public async Task LoginUser_WithNonExistentUser_ShouldReturnFailure()
        {
            _authRepositoryMock.Setup(repo => repo.LoginUser("nonexistent", "password123"))
                .ReturnsAsync(UserResult.Fail("Пользователь не найден или данные авторизации отсутствуют."));

            var result = await _authService.LoginUser("nonexistent", "password123");

            Assert.False(result.Succeeded);
            Assert.Null(result.Payload);
            Assert.Equal("Пользователь не найден или данные авторизации отсутствуют.", result.Error);
        }

        [Fact]
        public async Task LoginUser_WithIncorrectPassword_ShouldReturnFailure()
        {
            _authRepositoryMock
                .Setup(repo => repo.LoginUser("existingUser", "wrongpassword"))
                .ReturnsAsync(UserResult.Fail("Неверный пароль."));

            var result = await _authService.LoginUser("existingUser", "wrongpassword");

            Assert.False(result.Succeeded);
            Assert.Null(result.Payload);
            Assert.Equal("Неверный пароль.", result.Error);
        }
    }
}
