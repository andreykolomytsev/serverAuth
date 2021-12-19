using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("Permissions", Schema = "Identity")]
    public class ModelPermission
    {
        public string Id { get; set; }

        /// <summary>
        /// Тип права доступа
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public string ClaimValue { get; set; }


        /// <summary>
        /// Внешний ключ на таблицу с организациями
        /// </summary>
        public virtual ICollection<PermissionTenant> Tenants { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с ролями
        /// </summary>
        public virtual ICollection<PermissionRole> Roles { get; set; }
    }
}
