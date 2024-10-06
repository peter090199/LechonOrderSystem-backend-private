using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _configuration = configuration;
        }
        // PUT: api/Products/5
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] User data) // Accepts JSON input
        {
            if (id != data.Id)
            {
                return BadRequest("User ID mismatch.");
            }
            // Check if the employee exists in the database
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            // Update the existing employee's details
            user.UserName = data.UserName;
            user.Role = data.Role;
        //    user.Password = _passwordHasher.HashPassword(users, data.Password);

            // Mark the specific fields as modified
            _context.Entry(user).Property(u => u.UserName).IsModified = true;
            _context.Entry(user).Property(u => u.Role).IsModified = true;
         //   _context.Entry(user).Property(u => u.Password).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound("User not found.");
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
            return _context.Users.Any(e => e.Id == id);
        }
        // GET: api/Users
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
      
        //// GET: api/Products/5
        //[HttpGet("{userName}")]
        //public async Task<ActionResult<User>> GetUserName(string userName)
        //{
        //    // Search for the user by userName
        //    var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);

        //    // Check if the user was not found
        //    if (user == null)
        //    {
        //        return NotFound(new { message = "Username does not exist." });
        //    }

        //    // If the user exists, return the user object
        //    return Ok(user);
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
            {
                return BadRequest(new { message = "User already exists." });
            }

            // Hash the user's password
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            // Add new user to the database
            user.FirstName = "-";
            user.LastName = "-";
            user.BirthDate = DateTime.UtcNow;
            user.Email = "-";
            user.Address = "-";
            user.ContactNo = "0";
            user.MiddleName = "-";
            user.Role = "user";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
          
                // Find user in the database
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);

                if (existingUser == null)
                {
                    return Unauthorized(new { message = "Invalid credentials." });
                }
           
                // Verify the password
                var result = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.Password, user.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return Unauthorized(new { message = "Invalid credentials." });
                }
                var userRole = existingUser.Role;

                // Generate JWT token
                var token = GenerateJwtToken(existingUser);
                var assignedModules = new List<string>();

                if (userRole == "admin")
                {
                    assignedModules.Add("/header/dashboard"); // Add dashboard module only for admin
                }
                else
                {
                    assignedModules.Add("/header");
                }

            
                return Ok(new
                {
                    token,
                    role = userRole,
                    modules = assignedModules
                });
            
        }
    

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            if (string.IsNullOrEmpty(jwtIssuer))
            {
                throw new InvalidOperationException("JWT Issuer is not configured.");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role) // Include the user's role in the token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Set token expiration as needed
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "Employee not found." });
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Employee with ID {id} deleted successfully." });
        }

        [HttpGet("GetUserByName/{userName}")]
        public async Task<ActionResult<User>> GetUserByUsername(string userName)
        {
            // Convert both the database value and the input to lowercase for a case-insensitive comparison
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());

            // If user is not found, return 404
            if (user == null)
            {
                return NotFound($"User with username '{userName}' not found.");
            }

            // Return the found user
            return Ok(user);
        }


    }
}