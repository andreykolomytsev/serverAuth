using System.Collections.Generic;

namespace Application.DTOs.MicroService
{
    public class RequestServiceTenant
    {
        public List<TenantServiceIds> Tenants { get; set; }
    }

    public class TenantServiceIds
    {
        public string TenantId { get; set; }
    }
}
