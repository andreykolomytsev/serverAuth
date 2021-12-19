using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("PermissionRoles", Schema = "Identity")]
    public class PermissionRole
    {
        public string PermissionId { get; set; }
        public virtual ModelPermission Permission { get; set; }


        public string RoleId { get; set; }
        public virtual ModelRole Role { get; set; }
    }
}
