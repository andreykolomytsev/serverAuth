using System.Collections.Generic;

namespace Application.DTOs.MicroService
{
    public class ResponseServiceTenant
    {
        public string ServiceName { get; set; }

        public List<ResponseTenantService> Tenants { get; set; }
    }

    public class ResponseTenantService
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string INN { get; set; }

        public string OGRN { get; set; }

        public string KPP { get; set; }

        public string OKPO { get; set; }

        public bool Selected { get; set; }
    }
}
