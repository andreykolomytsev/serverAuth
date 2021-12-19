using System.Collections.Generic;

namespace Application.DTOs.Permissions
{
    public class RequestPermissionTenant
    {
        public IList<PermissionTenantIds> PermissionTenants { get; set; }
    }

    public class PermissionTenantIds
    {
        public string PermissionId { get; set; }
    }
}
