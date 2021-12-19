using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("Services", Schema = "Identity")]
    public class ModelService : IAuditableEntity<string>
    {
        public string Id { get; set; }

        /// <summary>
        /// Название сервиса
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// IP Адрес
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Порт
        /// </summary>
        public string Port { get; set; }

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
        /// Внешний ключ на таблицу с организациями
        /// </summary>
        public virtual ICollection<TenantService> Tenants { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с пользователями
        /// </summary>
        public virtual ICollection<UserService> Users { get; set; }
    }
}
