using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Users
{
    public class RequestUpdateUser
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

        public bool IsActive { get; set; } = false;

        public string TenantId { get; set; }
    }
}
