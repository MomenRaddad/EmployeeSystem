using Microsoft.AspNetCore.Mvc;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _svc;
        public DepartmentsController(IDepartmentService svc) => _svc = svc;

        [HttpGet]
        public IActionResult GetAll() => Ok(_svc.GetAll());
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) => _svc.GetById(id) is { } d ? Ok(d) : NotFound();

        [HttpPost]
        public IActionResult Create([FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = _svc.Create(input);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return _svc.Update(id, input) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _svc.Delete(id);
            if (!ok) return Conflict("Cannot delete department with existing employees.");
            return NoContent();
        }

        [HttpGet("{id:int}/employees")] public IActionResult EmployeesInDept(int id) => Ok(_svc.GetEmployees(id));
    }
}
