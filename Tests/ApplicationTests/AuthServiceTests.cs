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
            const string message = "The user has not been found or the auth data is missing";

            _authRepositoryMock.Setup(repo => repo.LoginUser("nonexistent", "password123"))
                .ReturnsAsync(UserResult.Fail(message));

            var result = await _authService.LoginUser("nonexistent", "password123");

            Assert.False(result.Succeeded);
            Assert.Null(result.Payload);
            Assert.Equal(message, result.Error);
        }

        [Fact]
        public async Task LoginUser_WithIncorrectPassword_ShouldReturnFailure()
        {
            const string message = "Wrong password";

            _authRepositoryMock
                .Setup(repo => repo.LoginUser("existingUser", "wrongpassword"))
                .ReturnsAsync(UserResult.Fail(message));

            var result = await _authService.LoginUser("existingUser", "wrongpassword");

            Assert.False(result.Succeeded);
            Assert.Null(result.Payload);
            Assert.Equal(message, result.Error);
        }
    }
}
