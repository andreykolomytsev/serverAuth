using Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("Users", Schema = "Identity")]
    public class ModelUser : IAuditableEntity<string>
    {
        public string Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Почта (логин)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Статус (Активный / Неактивный)
        /// </summary>
        public bool IsActive { get; set; }

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

        public virtual ICollection<TenantUser> Tenants { get; set; }

        /// <summary>
        /// Список токенов для обновления
        /// </summary>
        public virtual ICollection<ModelRefreshToken> RefreshTokens { get; set; }

        /// <summary>
        /// Список токенов доступа для управления
        /// </summary>
        public virtual ICollection<ModelAccessToken> AccessTokens { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с ролями
        /// </summary>
        public virtual ICollection<UserRole> Roles { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу с сервисами
        /// </summary>
        public virtual ICollection<UserService> UserServices { get; set; }
    }
}
