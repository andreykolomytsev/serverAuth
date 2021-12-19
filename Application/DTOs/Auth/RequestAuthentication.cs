using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class RequestAuthentication
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ServiceId { get; set; } = null;
    }
}
