using Application.DTOs.Tenants;
using System.Collections.Generic;

namespace Application.DTOs.MicroService
{
    public class ResponseServiceUser
    {
        public string ServiceName { get; set; }

        public List<ResponseUserService> Users { get; set; }
    }

    public class ResponseUserService
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public ResponseTenant Tenant { get; set; }

        public bool IsActive { get; set; }

        public bool Selected { get; set; }
    }
}
