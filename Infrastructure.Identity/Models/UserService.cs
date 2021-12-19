using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("UserServices", Schema = "Identity")]
    public class UserService
    {
        public string MicroServiceId { get; set; }
        public virtual ModelService MicroService { get; set; }


        public string UserId { get; set; }
        public virtual ModelUser User { get; set; }
    }
}
