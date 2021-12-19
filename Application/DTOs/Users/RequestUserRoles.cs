using System.Collections.Generic;

namespace Application.DTOs.Users
{
    public class RequestUserRoles
    {
        public IList<EditUserRoleModel> UserRoles { get; set; }
    }

    public class EditUserRoleModel
    {
        public string RoleId { get; set; }
    }
}
