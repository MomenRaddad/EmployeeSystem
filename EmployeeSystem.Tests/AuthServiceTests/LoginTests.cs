using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Models;
using EmployeeSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeSystem.Tests.AuthServiceTests;

public class LoginTests : TestBase
{
    // AUTH-LOG-001 | Priority: High
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock();
        var roleManagerMock = CreateRoleManagerMock();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var input = new LoginDto
        {
            Email = "user@example.com",
            Password = "Password123!"
        };

        var existingUser = new ApplicationUser
        {
            Email = input.Email,
            UserName = input.Email
        };

        userManagerMock
            .Setup(m => m.FindByEmailAsync(input.Email))
            .ReturnsAsync(existingUser);

        userManagerMock
            .Setup(m => m.CheckPasswordAsync(existingUser, input.Password))
            .ReturnsAsync(true);

        var expectedToken = new TokenResponseDto
        {
            Token = "jwt-token",
            Role = "Guest",
            Expiration = DateTime.UtcNow.AddHours(1)
        };

        jwtTokenServiceMock
            .Setup(j => j.GenerateTokenAsync(existingUser))
            .ReturnsAsync(expectedToken);

        var service = new AuthService(
            userManagerMock.Object,
            roleManagerMock.Object,
            jwtTokenServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await service.LoginAsync(input);

        // Assert — token returned
        Assert.NotNull(result);
        Assert.Equal(expectedToken.Token, result.Token);
        Assert.Equal(expectedToken.Role, result.Role);

        userManagerMock.Verify(m => m.FindByEmailAsync(input.Email), Times.Once);
        userManagerMock.Verify(m => m.CheckPasswordAsync(existingUser, input.Password), Times.Once);
        jwtTokenServiceMock.Verify(j => j.GenerateTokenAsync(existingUser), Times.Once);
    }
    // AUTH-LOG-002 | Priority: High

    [Fact]
    public async Task LoginAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock();
        var roleManagerMock = CreateRoleManagerMock();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var input = new LoginDto
        {
            Email = "notfound@example.com",
            Password = "Password123!"
        };

        // User not found
        userManagerMock
            .Setup(m => m.FindByEmailAsync(input.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var service = new AuthService(
            userManagerMock.Object,
            roleManagerMock.Object,
            jwtTokenServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await service.LoginAsync(input);

        Assert.Null(result);

        userManagerMock.Verify(m => m.FindByEmailAsync(input.Email), Times.Once);
        userManagerMock.Verify(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        jwtTokenServiceMock.Verify(j => j.GenerateTokenAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsNull()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock();
        var roleManagerMock = CreateRoleManagerMock();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var input = new LoginDto
        {
            Email = "user@example.com",
            Password = "WrongPassword!"
        };

        var existingUser = new ApplicationUser
        {
            Email = input.Email,
            UserName = input.Email
        };

        // user exists
        userManagerMock
            .Setup(m => m.FindByEmailAsync(input.Email))
            .ReturnsAsync(existingUser);

        // wrong password
        userManagerMock
            .Setup(m => m.CheckPasswordAsync(existingUser, input.Password))
            .ReturnsAsync(false);

        var service = new AuthService(
            userManagerMock.Object,
            roleManagerMock.Object,
            jwtTokenServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await service.LoginAsync(input);

        Assert.Null(result);

        
        userManagerMock.Verify(m => m.FindByEmailAsync(input.Email), Times.Once);
        userManagerMock.Verify(m => m.CheckPasswordAsync(existingUser, input.Password), Times.Once);
        jwtTokenServiceMock.Verify(j => j.GenerateTokenAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }
}
