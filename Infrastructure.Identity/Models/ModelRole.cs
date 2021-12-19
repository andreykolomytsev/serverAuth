using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("Roles", Schema = "Identity")]
    public class ModelRole : IAuditableEntity<string>
    {
        public ModelRole()
        {

        }

        public ModelRole(string name, string tenantId, string description = null)
        {
            Name = name;
            TenantId = tenantId;
            Description = description;
        }

        public string Id { get; set; }

        /// <summary>
        /// Название роли
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание роли
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Кем создан
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Когда создан
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Кем обновлен
        /// </summary>
        public string LastModifiedBy { get; set; }

        /// <summary>
        /// Когда обновлен
        /// </summary>
        public DateTime? LastModifiedOn { get; set; }

        /// <summary>
        /// Разделение видимости записей по арендаторам (фирмам)
        /// </summary>
        public string TenantId { get; set; }
        public virtual ModelTenant Tenant { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с пользователями
        /// </summary>
        public virtual ICollection<UserRole> Users { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с правами доступа
        /// </summary>
        public virtual ICollection<PermissionRole> Permissions { get; set; }
    }
}
