using BackendNETAPI.Data;
using BackendNETAPI.Model;
using BackendNETAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {

        private readonly MyDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserProfileController(MyDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }
        //private readonly IUserService _userService; // Assume you have a user service to handle user logic

        //public UserProfileController(IUserService userService)
        //{
        //    _userService = userService;
        //}

        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          //var result = await _context.SingleOrDefaultAsync(request.CurrentPassword, request.NewPassword);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
            {
                return NotFound("Password not found.");
            }
            if (user != null)
            {
                request.NewPassword = _passwordHasher.HashPassword(user,request.NewPassword);
                user.Password = request.NewPassword;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Password updated successfully." });
            }

            return BadRequest(new { message = "Failed to update password." });
        }


        [HttpPut("UpdateUser")]
        public async Task<IActionResult> PutUpdateUser([FromBody] User profileData) // Accepts JSON input
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == profileData.UserName);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            // Update the existing employee's details
            user.UserName = profileData.UserName;
            user.FirstName = profileData.FirstName;
            user.LastName = profileData.LastName;
            user.Email = profileData.Email;
            user.ContactNo = profileData.ContactNo;
            user.Address = profileData.Address;
            user.BirthDate = profileData.BirthDate;
            user.MiddleName = profileData.MiddleName;

           // user.Password = _passwordHasher.HashPassword(users, users.Password);
            _context.Entry(user).Property(u => u.UserName).IsModified = true;
            _context.Entry(user).Property(u => u.FirstName).IsModified = true;
            _context.Entry(user).Property(u => u.LastName).IsModified = true;
            _context.Entry(user).Property(u => u.Email).IsModified = true;
            _context.Entry(user).Property(u => u.ContactNo).IsModified = true;
            _context.Entry(user).Property(u => u.Address).IsModified = true;
            _context.Entry(user).Property(u => u.BirthDate).IsModified = true;
            _context.Entry(user).Property(u => u.MiddleName).IsModified = true;


            await _context.SaveChangesAsync();
           
            return Ok(new { message = "User successfully updated." });
        }

        [HttpGet("GetUserByUsername/{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            // Find the user by their username
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound($"User with username '{username}' not found.");
            }
            return Ok(user);
        }


        //private bool EmployeeExists(int id)
        //{
        //    return _context.Users.Any(e => e.Id == id);
        //}
        //public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileUpdateDto userProfile)
        //{
        //    if (userProfile == null || string.IsNullOrWhiteSpace(userProfile.Username))
        //    {
        //        return BadRequest("Invalid user data.");
        //    }

        //    var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == userProfile.Username);

        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    // Update the user properties
        //    user.FirstName = userProfile.FirstName;
        //    user.LastName = userProfile.LastName;
        //    user.Email = userProfile.Email;
        //    // Update other properties as necessary

        //    // Save changes to the database
        //    await _context.SaveChangesAsync();

        //    return Ok("Profile updated successfully.");
        //}
    }
}
public class UpdatePasswordRequest
{
    public string UserName { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
