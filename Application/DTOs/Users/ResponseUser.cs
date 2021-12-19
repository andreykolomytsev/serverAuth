using Application.DTOs.Tenants;

namespace Application.DTOs.Users
{
    public class ResponseUser
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public ResponseTenant Tenant { get; set; }

        public bool IsActive { get; set; }
    }
}
