using EmployeeSystem.Dtos;
using EmployeeSystem.Infrastructure;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(IEmployeeService svc, ILogger<EmployeesController> logger) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetAll() =>
             Ok(await svc.GetAll());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeModel>> GetById(int id)
        {
            var e = await svc.GetById(id);
            if (e is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Employee not found",
                    Detail = $"Employee {id} does not exist.",
                    Status = StatusCodes.Status404NotFound
                });
            }
            return Ok(e);
        }

        [HttpPost]
        [Authorize(Roles = nameof(AppRole.Admin))]
        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<ActionResult<EmployeeModel>> Create([FromBody] EmployeeModel input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model for CreateEmployee: {@Errors} {@input}", ModelState.ToSimpleErrors(), input);
                return ValidationProblem(ModelState);
            }

            try
            {
                var created = await svc.Create(input);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Department not found",
                    Detail = $"DepartmentId {input.DepartmentId} does not exist.",
                    Status = StatusCodes.Status404NotFound
                });
            }
        }
        [Authorize(Roles = nameof(AppRole.Admin))]
        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<IActionResult> Update(int id, [FromBody] EmployeeModel input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model for Update employee {EmployeeId}: {@Errors}", id, ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            try
            {
                var ok = await svc.Update(id, input);
                if (!ok)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Employee not found",
                        Detail = $"Employee {id} does not exist.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Department not found",
                    Detail = $"DepartmentId {input.DepartmentId} does not exist.",
                    Status = StatusCodes.Status404NotFound
                });
            }
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = nameof(AppRole.Admin))]
        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<IActionResult> Patch(int id, [FromBody] UpdateEmployeeDto input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model for Patch employee {EmployeeId}: {@Errors}", id, ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            var result = await svc.UpdatePartial(id, input);

            if (result.NotFound)
            {
                return NotFound(new { message = $"Employee {id} not found." });
            }

            if (!result.Success && result.Error is not null)
            {
                return BadRequest(new { message = result.Error });
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(AppRole.Admin))]

        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<IActionResult> Delete(int id)
        {
            var ok = await svc.Delete(id);
            if (!ok)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Employee not found",
                    Detail = $"Employee {id} does not exist.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return NoContent();
        }

        [HttpPost("filter")]
        [Authorize(Roles = nameof(AppRole.Admin))]
        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<ActionResult<IEnumerable<EmployeeModel>>> Filter([FromQuery] EmployeeFilter filter)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid query for FilterEmployees: {@Errors}", ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            var data = await svc.FilterEmployees(filter);
            return Ok(data);
        }

        [HttpPost("{id:int}/deactivate")]
        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<IActionResult> Deactivate(int id, [FromQuery] DateTime? endDate)
        {
            var ok = await svc.Deactivate(id, endDate);
            if (!ok)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
