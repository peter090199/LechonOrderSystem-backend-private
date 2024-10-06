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

        // GET: api/Products
        [HttpGet]   
        public async Task<ActionResult<IEnumerable<Products>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee); // Wrap the employee object in an Ok() result to ensure it returns as JSON
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Products employee) // Accepts JSON input
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

            return Ok(new { message = "Products successfully updated." });
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        // POST: api/Products
        [HttpPost("SavedEmployees")]
        public async Task<ActionResult<Products>> PostEmployee([FromBody] Products employee)
        {
            if (employee == null)
            {
                return BadRequest("Invalid employee data.");
            }

            try
            {
                if (employee.ImagePath != null && employee.ImagePath.Length > 0)
                {
                    // Generate a unique file name and save path
                    var fileName = Path.GetFileName(employee.ImagePath);
                    var directoryPath = Path.Combine("wwwroot", "images");
                    var filePath = Path.Combine(directoryPath, fileName);

                    // Ensure the directory exists
                    if (Directory.Exists(directoryPath) == false)
                    {
                        Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist
                    }

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                     //   await employee.ImagePath.CopyToAsync(stream);
                    }

                    // Save the image path in the database
                    employee.ImagePath = filePath; // Assuming you have ImagePath property in your Products model
                }
                 _context.Employees.Add(employee);

                // Save changes to the database
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully Saved." });
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