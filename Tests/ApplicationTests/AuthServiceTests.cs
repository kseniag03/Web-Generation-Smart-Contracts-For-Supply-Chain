using Application.Services;
using Core.Entities;
using Core.Interfaces;
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
        public async Task LoginUser_WithNonExistentUser_ShouldReturnNull()
        {
            _authRepositoryMock.Setup(repo => repo.GetByUsernameAsync("nonexistent"))
                .ReturnsAsync((User)null);

            var result = await _authService.LoginUser("nonexistent", "password123");

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUser_WithIncorrectPassword_ShouldReturnNull()
        {
            var user = new User
            {
                IdUser = 1,
                Login = "existingUser",
                Userauth = new Userauth
                {
                    IdUser = 1,
                    PasswordHash = "correctpassword"
                }
            };

            _authRepositoryMock.Setup(repo => repo.GetByUsernameAsync("existingUser"))
                .ReturnsAsync(user);

            var result = await _authService.LoginUser("existingUser", "wrongpassword");

            Assert.Null(result);
        }
    }
}
