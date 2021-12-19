using System.Collections.Generic;

namespace Application.DTOs.Permissions
{
    public class ResponsePermissionRole
    {
        public string RoleName { get; set; }

        public List<ResponsePermission> Permissions { get; set; }
    }
}
