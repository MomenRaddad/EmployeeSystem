using EmployeeSystem.Dtos;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(IEmployeeService svc) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> GetAll() =>
             Ok(await svc.GetAll());



        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeModel>> GetById(int id)
        {
            var e = await svc.GetById(id);
            return e is null ? NotFound() : Ok(e);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeModel>> Create([FromBody] EmployeeModel input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

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


        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var ok = await svc.Update(id, input);
                return ok ? NoContent() : NotFound($"Employee {id} does not exist.");
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
        public async Task<IActionResult> Patch(int id, [FromBody] UpdateEmployeeDto input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await svc.UpdatePartial(id, input);
            if (result.NotFound) return NotFound(new { message = $"Employee {id} not found." });
            if (!result.Success && result.Error is not null) return BadRequest(new { message = result.Error });

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) => (await svc.Delete(id)) ? NoContent() : NotFound();

        [HttpPost("filter")]

        public async Task<ActionResult<IEnumerable<EmployeeModel>>> Filter([FromQuery] EmployeeFilter filter)
        {
            if (!ModelState.IsValid)

                return BadRequest();

            return Ok(await svc.FilterEmployees(filter));

        }



        [HttpPost("{id:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int id, [FromQuery] DateTime? endDate) =>
             (await svc.Deactivate(id, endDate)) ? NoContent() : NotFound();
    }
}
