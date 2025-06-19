using GarageMasterBE.Models;
using GarageMasterBE.Services;
using Microsoft.AspNetCore.Mvc;

namespace GarageMasterBE.Controllers
{
    [ApiController]
    [Route("api/v1/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: api/v1/employees
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        // GET: api/v1/employees/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Employee>> GetById(string id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        // POST: api/v1/employees
        [HttpPost]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            await _employeeService.CreateAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        // PUT: api/v1/employees/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Employee updatedEmployee)
        {
            var existing = await _employeeService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            updatedEmployee.Id = existing.Id;
            updatedEmployee.UserId = existing.UserId; // Không cho phép thay đổi UserId
            var result = await _employeeService.UpdateAsync(id, updatedEmployee);

            if (!result) return BadRequest("Update failed.");
            return NoContent();
        }

        // DELETE: api/v1/employees/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _employeeService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var result = await _employeeService.DeleteAsync(id);
            if (!result) return BadRequest("Delete failed.");
            return NoContent();
        }

        // GET: api/v1/employees/search?name=abc
        [HttpGet("search")]
        public async Task<ActionResult<List<Employee>>> SearchByName([FromQuery] string name)
        {
            var employees = await _employeeService.SearchByNameAsync(name);
            return Ok(employees);
        }
    }
}
