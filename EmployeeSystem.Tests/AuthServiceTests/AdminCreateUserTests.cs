using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Models;
using EmployeeSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeSystem.Tests.AuthServiceTests;

public class AdminCreateUserTests : TestBase
{
    // AUTH-ADM-001 | Auth – AdminCreateUser | Priority: High
    [Fact]
    public async Task AdminCreateUserAsync_WithValidData_CreatesUserWithGivenRole()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock();
        var roleManagerMock = CreateRoleManagerMock();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var input = new AdminCreateUserDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            Role = AppRole.Admin  
        };

        ApplicationUser? createdUser = null;
        var roleName = input.Role?.ToString() ?? AppRole.Guest.ToString();

       
        userManagerMock
            .Setup(m => m.FindByEmailAsync(input.Email))
            .ReturnsAsync((ApplicationUser?)null);

       
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), input.Password))
            .Callback<ApplicationUser, string>((u, _) => createdUser = u)
            .ReturnsAsync(IdentityResult.Success);

      
        roleManagerMock
            .Setup(r => r.RoleExistsAsync(roleName))
            .ReturnsAsync(true);

      
        userManagerMock
            .Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), roleName))
            .ReturnsAsync(IdentityResult.Success);

        var service = CreateAuthService(
            userManagerMock.Object,
            roleManagerMock.Object,
            jwtTokenServiceMock.Object,
            loggerMock.Object
        );

        // Act
        var result = await service.AdminCreateUserAsync(input);

        
        Assert.NotNull(result);
        Assert.True(result.Succeeded);

       
        Assert.NotNull(createdUser);
        Assert.Equal(input.Email, createdUser!.Email);
        Assert.Equal(input.Email, createdUser.UserName);

        
        userManagerMock.Verify(m => m.FindByEmailAsync(input.Email), Times.Once);
        userManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), input.Password), Times.Once);
        roleManagerMock.Verify(r => r.RoleExistsAsync(roleName), Times.Once);
        userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), roleName), Times.Once);
    }

    // AUTH-ADM-002 | Priority: High
    [Fact]
    public async Task AdminCreateUserAsync_WithDuplicateEmail_ReturnsDuplicateError()
    {
        // Arrange
        var userManagerMock = CreateUserManagerMock();
        var roleManagerMock = CreateRoleManagerMock();
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        var loggerMock = new Mock<ILogger<AuthService>>();

        var input = new AdminCreateUserDto
        {
            Email = "existing@example.com",
            Password = "Password123!",
            Role = AppRole.Admin
        };

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
        var result = await service.AdminCreateUserAsync(input);

       
        Assert.NotNull(result);
        Assert.False(result.Succeeded);
        Assert.Contains(
            result.Errors,
            e => e.Code == "DuplicateEmail" && e.Description == $"User with email {input.Email} already exists."
        );

       
        userManagerMock.Verify(m => m.FindByEmailAsync(input.Email), Times.Once);
        userManagerMock.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        roleManagerMock.Verify(r => r.RoleExistsAsync(It.IsAny<string>()), Times.Never);
        userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }
}
