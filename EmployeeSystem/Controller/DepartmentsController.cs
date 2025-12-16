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

    public class DepartmentsController(IDepartmentService svc, ILogger<DepartmentsController> logger) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetAll()
            => Ok(await svc.GetAll());



        [HttpGet("{id:int}")]
        [Authorize(Roles = nameof(AppRole.Admin))]

        [SwaggerOperation(Summary = "Admin-only")]


        public async Task<ActionResult<DepartmentModel>> GetById(int id)
        {
            var d = await svc.GetById(id);
            if (d is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Department not found",
                    Detail = $"Department {id} does not exist.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(d);
        }

        [HttpPost]
        [Authorize(Roles = nameof(AppRole.Admin))]
        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<ActionResult<DepartmentModel>> Create([FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Invalid model for CreateDepartment: {@Errors}", ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            var created = await svc.Create(input);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(AppRole.Admin))]
        [SwaggerOperation(Summary = "Admin-only")]

        public async Task<IActionResult> Update(int id, [FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid)
            {
                logger.LogWarning(
                    "Invalid model for UpdateDepartment {DepartmentId}: {@Errors}",
                    id,
                    ModelState.ToSimpleErrors());
                return ValidationProblem(ModelState);
            }

            var ok = await svc.Update(id, input);
            if (!ok)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Department not found",
                    Detail = $"Department {id} does not exist.",
                    Status = StatusCodes.Status404NotFound
                });
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

                return Conflict(new ProblemDetails
                {
                    Title = "Cannot delete department",
                    Detail = "Cannot delete department because it either does not exist or has employees.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            return NoContent();
        }

        [HttpGet("{id:int}/employees")]
        [Authorize(Roles = nameof(AppRole.User) + nameof(AppRole.Admin))]
        [SwaggerOperation(Summary = "user,admin")]

        public async Task<ActionResult<IEnumerable<EmployeeModel>>> EmployeesInDept(int id)
            => Ok(await svc.GetEmployees(id));
    }
}
