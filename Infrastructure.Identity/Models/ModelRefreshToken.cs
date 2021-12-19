using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("RefreshTokens", Schema = "Identity")]
    public class ModelRefreshToken
    {
        public string Id { get; set; }

        /// <summary>
        /// Токен обновления
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Создан
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Истекает
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Создан по IP
        /// </summary>
        public string CreatedByIp { get; set; }

        /// <summary>
        /// Создан через браузер
        /// </summary>
        public string CreatedByBrowser { get; set; }

        /// <summary>
        /// Когда токен был отозван
        /// </summary>
        public DateTime? Revoked { get; set; }

        /// <summary>
        /// Отозван по IP
        /// </summary>
        public string RevokedByIp { get; set; }

        /// <summary>
        /// Новый токен обновления
        /// </summary>
        public string ReplacedByToken { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу пользователей
        /// </summary>
        public string UserId { get; set; }
        public virtual ModelUser User { get; set; }

        /// <summary>
        /// Вспомогательный метод, для установления просрочен ли токен
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public ModelRefreshToken()
        {

        }

        public ModelRefreshToken(ModelUser user)
        {
            User = user;
        }
    }
}
