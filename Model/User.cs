using System.ComponentModel.DataAnnotations;

namespace BackendNETAPI.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

       
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "user"; // Default role is "user"

        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // Nullable string

        private DateTime _birthDate;

        public DateTime BirthDate
        {
            get => TimeZoneInfo.ConvertTimeToUtc(_birthDate, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
            set => _birthDate = TimeZoneInfo.ConvertTimeFromUtc(value, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
        }

        public string ContactNo { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

    }
}
