using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ModulesController(MyDbContext context)
        {
            _context = context;
        }


        // GET: api/<ModulesController>
        [HttpGet("GetModules")]
        public async Task<ActionResult<IEnumerable<Modules>>> GetModules()
        {
            // Fetch modules from the database and sort them, with 'Dashboard' and 'Files' first, then alphabetically
            var sortedModules = await _context.Modules
                .OrderBy(m => m.ModuleName.ToLower() == "Dashboard" ? 0 : (m.ModuleName.ToLower() == "Files" ? 1 : 2))  // Prioritize 'Dashboard' and 'Files'
                .ThenBy(m => m.ModuleName)  // Sort the rest alphabetically by name
                .ToListAsync();

            return Ok(sortedModules);  // Return the sorted list
        }




        // GET api/<ModulesController>/5
        [HttpGet("GetModuleByID/{id}")]
        public async Task<ActionResult<Modules>> GetModuleByID(int id)
        {
            var employee = await _context.Modules.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee); // Wrap the employee object in an Ok() result to ensure it returns as JSON
        }


        // POST: api/Employees
        [HttpPost("SavedModules")]
        public async Task<ActionResult<Modules>> SaveModules([FromBody] Modules m)
        {
            if (m == null)
            {
                return BadRequest("Invalid module data.");
            }

            try
            {
                m.RecordStatus = "Active";
                _context.Modules.Add(m);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfuly Saved." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving the modules." + ex.Message);
            }

        }


        // PUT api/<ModulesController>/5
        [HttpPut("UpdateModule/{id}")]
        public async Task<IActionResult> PutModule(int id, [FromBody] Modules m) // Accepts JSON input
        {
            if (id != m.Id)
            {
                return BadRequest("Module ID mismatch.");
            }
            // Check if the module exists in the database
            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound("Module not found.");
            }
            // Update the existing module's details
             m.RecordStatus = "Active";
            _context.Entry(module).CurrentValues.SetValues(m);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    return NotFound("Module not found.");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Modules successfully updated." });
        }

        private bool Exists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }


        // DELETE api/<ModulesController>/5
        [HttpDelete("DeleteModule/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var module = await _context.Modules.FirstOrDefaultAsync(x => x.Id == id);
            if (module == null)
            {
                return NotFound(new { message = "Modules not found." });
            }
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Modules with ID {id} deleted successfully." });
        }

    }
}
