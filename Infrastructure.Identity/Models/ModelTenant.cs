using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("Tenants", Schema = "Identity")]
    public class ModelTenant : IAuditableEntity<string>
    {
        public string Id { get; set; }

        /// <summary>
        /// Полное наименование организации
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Адрес электронной почты
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string INN { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        public string OGRN { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        public string KPP { get; set; }

        /// <summary>
        /// ОКПО
        /// </summary>
        public string OKPO { get; set; }

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
        /// Ссылка на таблицу пользователей
        /// </summary>
        public virtual ICollection<TenantUser> Users { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с правами доступа
        /// </summary>
        public virtual ICollection<PermissionTenant> Permissions { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с сервисами
        /// </summary>
        public virtual ICollection<TenantService> Services { get; set; }

        public string TenantId { get; set; }
        public virtual ModelTenant Tenant { get; set; }
    }
}
