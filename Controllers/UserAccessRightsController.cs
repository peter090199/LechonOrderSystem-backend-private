using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccessRightsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UserAccessRightsController(MyDbContext context)
        {
            _context = context;
        }

        // POST: api/AccessRights
        [HttpPost]
        public async Task<ActionResult<UserAccessrights>> PostAccessRights([FromBody] UserAccessrights access)
        {
            if (access == null)
            {
                return BadRequest("Invalid Access data.");
            }
            try
            {
                access.RecordStatus = "Active";
                _context.UserAccessrights.Add(access);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfuly Saved." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving the Access Rights." + ex.Message);
            }

        }

        // GET: api/GetAccessRights
        [HttpGet("GetAccessRights")]
        public async Task<ActionResult<IEnumerable<UserAccessrights>>> AccessRights()
        {
            return await _context.UserAccessrights.ToListAsync();
        }

        [HttpDelete("DeleteAccess/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var access = await _context.UserAccessrights.FirstOrDefaultAsync(x => x.Id == id);
            if (access == null)
            {
                return NotFound(new { message = "Access not found." });
            }
            _context.UserAccessrights.Remove(access);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Employee with ID {id} deleted successfully." });
        }

    }
}
