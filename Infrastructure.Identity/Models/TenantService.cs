using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("TenantServices", Schema = "Identity")]
    public class TenantService
    {
        public string MicroServiceId { get; set; }
        public virtual ModelService MicroService { get; set; }


        public string TenantId { get; set; }
        public virtual ModelTenant Tenant { get; set; }
    }
}
