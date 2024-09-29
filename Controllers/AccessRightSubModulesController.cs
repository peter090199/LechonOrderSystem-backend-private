using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessRightSubModulesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public AccessRightSubModulesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/<AccessRightSubModules>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessRightSubModule>>> GetAccessRightSubModules()
        {
            return await _context.AccessRightSubModules.ToListAsync();
        }

        // GET api/<AccessRightSubModules>/5
        [HttpGet("{id}")]
        public string GetAccessRightSubModules(int id)
        {
            return "value";
        }

        // POST api/<AccessRightSubModules>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AccessRightSubModules>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccessRightSubModules>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
