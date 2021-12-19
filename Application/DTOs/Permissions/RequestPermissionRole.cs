using System.Collections.Generic;

namespace Application.DTOs.Permissions
{
    public class RequestPermissionRole
    {
        public IList<PermissionRoleIds> PermissionRoles { get; set; }
    }

    public class PermissionRoleIds
    {
        public string PermissionId { get; set; }
    }
}
