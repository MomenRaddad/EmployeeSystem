using EmployeeSystem.Dtos.Auth;
using EmployeeSystem.Infrastructure;
using EmployeeSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService,
        ILogger<AuthController> logger) : ControllerBase
    {
                [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Register new user (self-register, default role = Guest)")]
        public async Task<IActionResult> Register([FromBody] RegisterDto input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model for Register: {@Errors}", ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            var result = await authService.RegisterAsync(input);

            if (!result.Succeeded)
            {
                var errorDescriptions = result.Errors.Select(e => e.Description).ToArray();

                if (result.Errors.Any(e => e.Code == "DuplicateEmail"))
                {
                    return Conflict(new ProblemDetails
                    {
                        Title = "Email already in use",
                        Detail = string.Join("; ", errorDescriptions),
                        Status = StatusCodes.Status409Conflict
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Title = "User registration failed",
                    Detail = string.Join("; ", errorDescriptions),
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new
            {
                message = "User registered successfully"
            });
        }

                [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Login and get JWT token")]
        public async Task<IActionResult> Login([FromBody] LoginDto input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model for Login: {@Errors}", ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            var tokenDto = await authService.LoginAsync(input);

            if (tokenDto is null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid credentials",
                    Detail = "Email or password is incorrect.",
                    Status = StatusCodes.Status401Unauthorized
                });
            }

            return Ok(tokenDto);
        }

                [HttpPost("admin/create-user")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Admin-only")]
        public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserDto input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model for AdminCreateUser: {@Errors}", ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            var result = await authService.AdminCreateUserAsync(input);

            if (!result.Succeeded)
            {
                var errorDescriptions = result.Errors.Select(e => e.Description).ToArray();

                if (result.Errors.Any(e => e.Code == "DuplicateEmail"))
                {
                    return Conflict(new ProblemDetails
                    {
                        Title = "Email already in use",
                        Detail = string.Join("; ", errorDescriptions),
                        Status = StatusCodes.Status409Conflict
                    });
                }

                if (result.Errors.Any(e => e.Code == "RoleNotFound"))
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Role does not exist",
                        Detail = string.Join("; ", errorDescriptions),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                return BadRequest(new ProblemDetails
                {
                    Title = "Admin create user failed",
                    Detail = string.Join("; ", errorDescriptions),
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Ok(new
            {
                message = "User created successfully by admin",
                email = input.Email,
                role = string.IsNullOrWhiteSpace(input.Role.ToString()) ? AppRole.Guest.ToString() : input.Role.ToString()!.Trim()
            });
        }
    }
}
