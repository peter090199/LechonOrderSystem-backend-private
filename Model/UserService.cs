//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity; // For UserManager
//using BackendNETAPI.Data; // Adjust based on your actual data context namespace

//namespace BackendNETAPI.Models;
//public class UserService : IUserService
//{
//    private readonly UserManager<ApplicationUser> _userManager; // Assuming you have an ApplicationUser class
//    private readonly MyDbContext _dbContext; // Replace with your actual DbContext

//    public UserService(UserManager<ApplicationUser> userManager, MyDbContext dbContext)
//    {
//        _userManager = userManager;
//        _dbContext = dbContext;
//    }

//    public async Task<bool> UpdatePasswordAsync(string userId, string currentPassword, string newPassword)
//    {
//        // Find the user by userId
//        var user = await _userManager.FindByIdAsync(userId);
//        if (user == null)
//        {
//            return false; // User not found
//        }

//        // Validate current password
//        var passwordValid = await _userManager.CheckPasswordAsync(user, currentPassword);
//        if (!passwordValid)
//        {
//            return false; // Current password is incorrect
//        }

//        // Update the password
//        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
//        if (result.Succeeded)
//        {
//            // Password updated successfully
//            await _dbContext.SaveChangesAsync(); // Save changes if necessary
//            return true;
//        }

//        // Handle errors
//        foreach (var error in result.Errors)
//        {
//            // Log the error or handle it as needed
//            // Example: Console.WriteLine(error.Description);
//        }

//        return false; // Return false if update failed
//    }
//}
