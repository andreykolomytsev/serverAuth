using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("UserRoles", Schema = "Identity")]
    public class UserRole
    {
        public string UserId { get; set; }
        public virtual ModelUser User { get; set; }


        public string RoleId { get; set; }
        public virtual ModelRole Role { get; set; }
    }
}
