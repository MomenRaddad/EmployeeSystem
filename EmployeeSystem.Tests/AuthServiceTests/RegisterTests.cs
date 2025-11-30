using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Models;
using EmployeeSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeSystem.Tests.AuthServiceTests;

public class RegisterTests : TestBase
{
    // Test ID: AUTH-REG-001 | Priority: High
    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUserWithGuestRole()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock();
        var roleManagerMock = CreateRoleManagerMock();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        
        var input = AuthTestHelper.CreateValidRegisterDto("newuser@example.com");

        ApplicationUser? createdUser = null;
        var guestRoleName = AppRole.Guest.ToString();

        
        userManagerMock
            .Setup(m => m.FindByEmailAsync(input.Email))
            .ReturnsAsync((ApplicationUser?)null);

       
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), input.Password))
            .Callback<ApplicationUser, string>((u, _) => createdUser = u)
            .ReturnsAsync(IdentityResult.Success);

        roleManagerMock
            .Setup(r => r.RoleExistsAsync(guestRoleName))
            .ReturnsAsync(true);

        userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), guestRoleName))
            .ReturnsAsync(IdentityResult.Success);

        var service = CreateAuthService(
            userManagerMock.Object,
            roleManagerMock.Object,
            jwtTokenServiceMock.Object,
            loggerMock.Object
        );


        // Act
        var result = await service.RegisterAsync(input);

        // result
        Assert.NotNull(result);
        Assert.True(result.Succeeded);

        // created user
        Assert.NotNull(createdUser);
        Assert.Equal(input.Email, createdUser.Email);

        // calls to UserManager
        userManagerMock.Verify(m => m.FindByEmailAsync(input.Email), Times.Once);
        userManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), input.Password), Times.Once);
        roleManagerMock.Verify(r => r.RoleExistsAsync(guestRoleName), Times.Once);
        userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), guestRoleName), Times.Once);
    }
    // Test ID: AUTH-REG-002 | Priority: High
    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsDuplicateError()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock();
        var roleManagerMock = CreateRoleManagerMock();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var input = AuthTestHelper.CreateValidRegisterDto("existing@example.com");

        var existingUser = new ApplicationUser
        {
            Email = input.Email,
            UserName = input.Email
        };

        userManagerMock
            .Setup(m => m.FindByEmailAsync(input.Email))
            .ReturnsAsync(existingUser);

        var service = CreateAuthService(
            userManagerMock.Object,
            roleManagerMock.Object,
            jwtTokenServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await service.RegisterAsync(input);

        // Assert – failed result
        Assert.NotNull(result);
        Assert.False(result.Succeeded);

        Assert.Contains(result.Errors, e => e.Description == $"A user with email {input.Email} already exists.");
        
        userManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);

        roleManagerMock.Verify(r => r.RoleExistsAsync(It.IsAny<string>()), Times.Never);

        userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }


}
