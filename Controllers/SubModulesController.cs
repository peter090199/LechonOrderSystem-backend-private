using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubModulesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public SubModulesController(MyDbContext context)
        {
            _context = context;
        }


        // GET: api/<ModulesController>
        [HttpGet("GetSubModules")]
        public async Task<ActionResult<IEnumerable<SubModule>>> GetSubModules()
        {
            return await _context.SubModules.ToListAsync();
        }


        // GET api/<SubModulesController>/5
        [HttpGet("GetSubModuleById/{moduleId}")]
        public async Task<ActionResult<List<SubModule>>> GetSubModuleById([FromRoute] int moduleId)
        {
            var subModules = await _context.SubModules
                        .Where(sm => sm.ModuleId == moduleId)
                        .ToListAsync();

            if (!subModules.Any()) // Check if the list is empty instead of null
            {
                return NotFound(); // Return 404 if not found
            }

            return Ok(subModules); // Return 200 with the list of data
        }


        // POST api/<SubModulesController>
        [HttpPost("SaveSubModule")]
        public async Task<ActionResult<SubModule>> SaveModules([FromBody] SubModule data)
        {
            if (data == null)
            {
                return BadRequest("Invalid sub-module data.");
            }

            try
            {
                data.RecordStatus = "Active";
                data.Route = "";
                _context.SubModules.Add(data);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully Saved." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving the sub-modules." + ex.Message);
            }

        }

        // PUT api/<SubModulesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SubModulesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
