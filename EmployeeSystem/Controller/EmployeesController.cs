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
        public IActionResult GetAll() => Ok(svc.GetAll());
        [HttpGet("active")]
        public IActionResult GetActive() => Ok(svc.GetActive());
        [HttpGet("inactive")]
        public IActionResult GetInactive() => Ok(svc.GetInactive());
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) => svc.GetById(id) is { } e ? Ok(e) : NotFound();

        [HttpPost]
        public IActionResult Create([FromBody] EmployeeModel input)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState); 

            try
            {
                var created = svc.Create(input);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
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
        public IActionResult Update(int id, [FromBody] EmployeeModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var updated = svc.Update(id, input);
                return updated ? NoContent() : NotFound($"The user {id} does not exist.");

            }
            catch (InvalidOperationException ex)
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
        public IActionResult Patch(int id, [FromBody] UpdateEmployeeDto input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = svc.UpdatePartial(id, input);

            if (result.NotFound)
                return NotFound(new { message = $"Employee {id} not found." });

            if (!result.Success && result.Error is not null)
                return BadRequest(new { message = result.Error });

            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) => svc.Delete(id) ? NoContent() : NotFound();

        [HttpGet("by-department")]
        public IActionResult ByDept([FromQuery]int departmentId) => Ok(svc.GetByDepartmentId(departmentId));

        [HttpGet("by-position")]
        public IActionResult ByPosition([FromQuery] string position) => Ok(svc.GetByPosition(position));

        [HttpGet("min-years")]
        public IActionResult MinYears([FromQuery] int minYears) => Ok(svc.GetWithMinYears(minYears));

        [HttpPost("{id:int}/deactivate")]
        public IActionResult Deactivate(int id, [FromQuery] DateTime endDate) => svc.Deactivate(id, endDate) ? NoContent() : NotFound();
    }
}
