using System.Collections.Generic;

namespace Application.DTOs.Users
{
    public class ResponseUserRoles
    {
        public List<UserRoleModel> UserRoles { get; set; }
    }

    public class UserRoleModel
    {
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

        public bool Selected { get; set; } = false;
    }
}
