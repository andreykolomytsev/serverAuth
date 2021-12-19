using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("PermissionTenants", Schema = "Identity")]
    public class PermissionTenant
    {
        public string PermissionId { get; set; }
        public virtual ModelPermission Permission { get; set; }


        public string TenantId { get; set; }
        public virtual ModelTenant Tenant { get; set; }
    }
}
