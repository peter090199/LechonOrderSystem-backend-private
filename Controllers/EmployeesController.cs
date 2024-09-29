using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public EmployeesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]   
        public async Task<ActionResult<IEnumerable<Employees>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employees>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee); // Wrap the employee object in an Ok() result to ensure it returns as JSON
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Employees employee) // Accepts JSON input
        {
            if (id != employee.Id)
            {
                return BadRequest("Employee ID mismatch.");
            }
            // Check if the employee exists in the database
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound("Employee not found.");
            }
            // Update the existing employee's details
            _context.Entry(existingEmployee).CurrentValues.SetValues(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound("Employee not found.");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Employees successfully updated." });
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        // POST: api/Employees
        [HttpPost("SavedEmployees")]
        public async Task<ActionResult<Employees>> PostEmployee([FromBody] Employees employee)
        {
            if (employee == null)
            {
                return BadRequest("Invalid employee data.");
            }

            try
            {
                // Add the employee to the context
                _context.Employees.Add(employee);

                // Save changes to the database
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfuly Saved." });
                // Return the created employee with a 201 status code
              //  return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                // Log the error (not shown here) and return a generic error message
                return StatusCode(500, "An error occurred while saving the employee." + ex.Message);
            }

        }


        [HttpDelete("{empID}")]
        public async Task<IActionResult> DeleteEmployee(string empID)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.EmpID == empID);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found." });
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Employee with ID {empID} deleted successfully." });
        }


    }
}