using System.Collections.Generic;

namespace Application.DTOs.Permissions
{
    public class ResponsePermissionTenant
    {
        public string TenantName { get; set; }

        public List<ResponsePermission> Permissions { get; set; }
    }
}
