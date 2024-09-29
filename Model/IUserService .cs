using System.Threading.Tasks;

namespace BackendNETAPI.Models
{
    public interface IUserService
    {
        Task<bool> UpdatePasswordAsync(string currentPassword, string newPassword);
        // You can add more user-related methods here
    }
}
