using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace EmployeeSystem.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IJwtTokenService jwtTokenService,
    ILogger<AuthService> logger
) : IAuthService
{
    public async Task<IdentityResult> RegisterAsync(RegisterDto input)
    {
        logger.LogInformation("[AUTH] Register request for {Email}", input.Email);

        var existing = await userManager.FindByEmailAsync(input.Email);
        if (existing is not null)
        {
            logger.LogWarning("[AUTH] Register failed for {Email}: Duplicate email", input.Email);

            return IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = $"A user with email {input.Email} already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = input.Email,
            Email = input.Email
        };

        var create = await userManager.CreateAsync(user, input.Password);



        if (!create.Succeeded)
        {
            logger.LogWarning(
                "[AUTH] Failed creating user {Email}: {Errors}",
                input.Email,
                string.Join("; ", create.Errors.Select(e => e.Description)));

            return create;
        }

        string roleName = AppRole.Guest.ToString();

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            logger.LogInformation("[AUTH] Role {Role} not found, creating it...", roleName);

            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded)
            {
                logger.LogWarning(
                    "[AUTH] Failed creating default role {Role}: {Errors}",
                    roleName,
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));

                return IdentityResult.Failed(roleResult.Errors.ToArray());
            }
        }

        var addRole = await userManager.AddToRoleAsync(user, roleName);
        if (!addRole.Succeeded)
        {
            logger.LogWarning(
                "[AUTH] Failed assigning role {Role} to {Email}: {Errors}",
                roleName,
                input.Email,
                string.Join("; ", addRole.Errors.Select(e => e.Description)));

            return addRole;
        }
        Console.WriteLine(roleName);
        logger.LogInformation("[AUTH] User {Email} registered with role {Role}", input.Email, roleName);

        return IdentityResult.Success;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto input)
    {
        logger.LogInformation("[AUTH] Login request for {Email} ", input.Email);

        var user = await userManager.FindByEmailAsync(input.Email);
        if (user is null)
        {
            logger.LogWarning("[AUTH] Login failed for {Email}: User not found", input.Email);
            return null;
        }

        var valid = await userManager.CheckPasswordAsync(user, input.Password);
        if (!valid)
        {
            logger.LogWarning("[AUTH] Login failed for {Email}: Invalid password", input.Email);
            return null;
        }

        var token = await jwtTokenService.GenerateTokenAsync(user);

        logger.LogInformation("[AUTH] User {Email} logged in successfully with role {Role}", input.Email, token.Role);

        return token;
    }

    public async Task<IdentityResult> AdminCreateUserAsync(AdminCreateUserDto input)
    {
        logger.LogInformation("[AUTH] AdminCreateUser request for {Email} with role {Role}",
            input.Email,
            string.IsNullOrWhiteSpace(input.Role.ToString()) ? AppRole.Guest.ToString() : input.Role);

        var existing = await userManager.FindByEmailAsync(input.Email);
        if (existing is not null)
        {
            logger.LogWarning("[AUTH] AdminCreateUser failed for {Email}: Duplicate email", input.Email);

            return IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = $"User with email {input.Email} already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = input.Email,
            Email = input.Email
        };

        var create = await userManager.CreateAsync(user, input.Password);
        if (!create.Succeeded)
        {
            logger.LogWarning(
                "[AUTH] Admin failed creating user {Email}: {Errors}",
                input.Email,
                string.Join("; ", create.Errors.Select(e => e.Description)));

            return create;
        }

        var roleName = string.IsNullOrWhiteSpace(input.Role.ToString()) ? AppRole.Guest.ToString() : input.Role.ToString().Trim();

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            logger.LogWarning("[AUTH] Admin failed, role {Role} does not exist", roleName);

            return IdentityResult.Failed(new IdentityError
            {
                Code = "RoleNotFound",
                Description = $"Role '{roleName}' does not exist."
            });
        }

        var addRole = await userManager.AddToRoleAsync(user, roleName);
        if (!addRole.Succeeded)
        {
            logger.LogWarning(
                "[AUTH] Failed assigning role {Role} to user {Email}: {Errors}",
                roleName,
                input.Email,
                string.Join("; ", addRole.Errors.Select(e => e.Description)));

            return addRole;
        }

        logger.LogInformation("[AUTH] Admin created user {Email} with role {Role}", input.Email, roleName);

        return IdentityResult.Success;
    }
}
