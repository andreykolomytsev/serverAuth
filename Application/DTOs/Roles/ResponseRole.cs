using Application.DTOs.Tenants;

namespace Application.DTOs.Roles
{
    public class ResponseRole
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ResponseTenant Tenant { get; set; }
    }
}
