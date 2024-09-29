using Microsoft.AspNetCore.Identity;

namespace BackendNETAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add any additional properties here
        public string FirstName { get; set; }=string.Empty;
        public string LastName { get; set; } = string.Empty;
        // Additional properties can be added as required
    }
}
