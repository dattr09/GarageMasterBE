using GarageMasterBE.Models;
using GarageMasterBE.Services;
using Microsoft.AspNetCore.Mvc;

namespace GarageMasterBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        // GET: api/Customers/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Customer>> GetById(string id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found." });

            return Ok(customer);
        }

        // GET: api/Customers/search?name=abc
        [HttpGet("search")]
        public async Task<ActionResult<List<Customer>>> GetByName([FromQuery] string name)
        {
            var customers = await _customerService.GetByNameAsync(name);
            return Ok(customers);
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> Create(Customer customer)
        {
            await _customerService.CreateAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        // PUT: api/Customers/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Customer updatedCustomer)
        {
            var existingCustomer = await _customerService.GetByIdAsync(id);
            if (existingCustomer == null)
                return NotFound(new { message = "Customer not found." });

            updatedCustomer.Id = id; // Giữ nguyên ID
            var result = await _customerService.UpdateAsync(id, updatedCustomer);

            return result ? NoContent() : StatusCode(500, new { message = "Update failed." });
        }

        // DELETE: api/Customers/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingCustomer = await _customerService.GetByIdAsync(id);
            if (existingCustomer == null)
                return NotFound(new { message = "Customer not found." });

            var result = await _customerService.DeleteAsync(id);
            return result ? NoContent() : StatusCode(500, new { message = "Delete failed." });
        }
    }
}
