using Microsoft.AspNetCore.Mvc;
using EmployeeSystem.Models;
using EmployeeSystem.Services.Interfaces;

namespace EmployeeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController(IDepartmentService svc) : ControllerBase
    {


        [HttpGet]
        public IActionResult GetAll() => Ok(svc.GetAll());
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) => svc.GetById(id) is { } d ? Ok(d) : NotFound();

        [HttpPost]
        public IActionResult Create([FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = svc.Create(input);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return svc.Update(id, input) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = svc.Delete(id);
            if (!ok) return Conflict("Cannot delete department with existing employees.");
            return NoContent();
        }

        [HttpGet("{id:int}/employees")] public IActionResult EmployeesInDept(int id) => Ok(svc.GetEmployees(id));
    }
}
