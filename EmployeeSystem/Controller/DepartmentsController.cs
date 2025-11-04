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
        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetAll()
            => Ok(await svc.GetAll()); 

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentModel>> GetById(int id)
        {
            var d = await svc.GetById(id);
            return d is null ? NotFound() : Ok(d);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentModel>> Create([FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await svc.Create(input);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentModel input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return await svc.Update(id, input) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await svc.Delete(id);
            if (!ok) return Conflict("Cannot delete department with existing employees.");
            return NoContent();
        }

        [HttpGet("{id:int}/employees")]
        public async Task<ActionResult<IEnumerable<EmployeeModel>>> EmployeesInDept(int id)
            => Ok(await svc.GetEmployees(id));
    }
}
