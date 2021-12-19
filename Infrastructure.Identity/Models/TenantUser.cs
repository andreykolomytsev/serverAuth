using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("TenantUsers", Schema = "Identity")]
    public class TenantUser
    {
        public string TenantId { get; set; }
        public virtual ModelTenant Tenant { get; set; }


        public string UserId { get; set; }
        public virtual ModelUser User { get; set; }
    }
}
