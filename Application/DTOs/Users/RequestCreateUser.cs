using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Users
{
    public class RequestCreateUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string MiddleName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public bool IsActive { get; set; } = false;

        public string TenantId { get; set; }
    }
}
