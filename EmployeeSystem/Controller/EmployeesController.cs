using Microsoft.AspNetCore.Mvc;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _svc;
        public EmployeesController(IEmployeeService svc) => _svc = svc;

        [HttpGet]
        public IActionResult GetAll() => Ok(_svc.GetAll());
        [HttpGet("active")]
        public IActionResult GetActive() => Ok(_svc.GetActive());
        [HttpGet("inactive")]
        public IActionResult GetInactive() => Ok(_svc.GetInactive());
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) => _svc.GetById(id) is { } e ? Ok(e) : NotFound();

        [HttpPost]
        public IActionResult Create([FromBody] EmployeeModel input)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState); 

            try
            {
                var created = _svc.Create(input);
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
            return _svc.Update(id, input) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) => _svc.Delete(id) ? NoContent() : NotFound();

        [HttpGet("by-department/{departmentId:int}")]
        public IActionResult ByDept(int departmentId) => Ok(_svc.GetByDepartmentId(departmentId));

        [HttpGet("by-position/{position}")]
        public IActionResult ByPosition(string position) => Ok(_svc.GetByPosition(position));

        [HttpGet("min-years/{minYears:int}")]
        public IActionResult MinYears(int minYears) => Ok(_svc.GetWithMinYears(minYears));

        [HttpPost("{id:int}/deactivate")]
        public IActionResult Deactivate(int id, [FromQuery] DateTime endDate) => _svc.Deactivate(id, endDate) ? NoContent() : NotFound();
    }
}
