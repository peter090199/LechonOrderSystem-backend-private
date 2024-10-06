using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;
  

        public ProductsController(MyDbContext context)
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

        [HttpGet("GetProductByImageName/{filename}")]
        public async Task<ActionResult<Products>> GetUserByUsername(string filename)
        {
            // Find the user by their username
            var pr = await _context.Employees
                                     .FirstOrDefaultAsync(u => u.EmpName == filename);
            if (pr == null)
            {
                return NotFound($"Product with username '{filename}' not found.");
            }
            return Ok(pr);
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
        public async Task<ActionResult<Products>> PostEmployee([FromBody] Products products)
        {
            if (products == null)
            {
                return BadRequest("Invalid employee data.");
            }

            try
            {
                 _context.Employees.Add(products);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully Saved." });
            }
            catch (Exception ex)
            {
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

        // POST: api/Products/SavedEmployees
        [HttpPost("SavedProducts")]
        public async Task<IActionResult> SaveEmployee([FromForm] Products products, [FromForm] IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                //// Create the directory to store images if it doesn't exist
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                // Save image file to server
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

              //  Save product/ employee data to the database
                var data = new Products
                {
                    EmpID = products.EmpID,
                    EmpName = products.EmpName,
                    Address = products.Address,
                    ContactNo = products.ContactNo,
                    ImagePath = fileName
                };
                _context.Employees.Add(data);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Product saved successfully" });

            }

            return BadRequest("Image upload failed.");
        }



    }
}