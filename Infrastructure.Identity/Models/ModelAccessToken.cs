using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Identity.Models
{
    [Table("AccessTokens", Schema = "Identity")]
    public class ModelAccessToken
    {
        public string Id { get; set; }

        /// <summary>
        /// Токен доступа
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
        /// Статус (Если активный - вход в систему разрешен)
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Флаг для обновления (если нужно обновить токен)
        /// </summary>
        public bool IsOutDated { get; set; }

        /// <summary>
        /// Внешний ключ на таблицу пользователей
        /// </summary>
        public string UserId { get; set; }
        public virtual ModelUser User { get; set; }

        /// <summary>
        /// Вспомогательный метод, для установления просрочен ли токен
        /// </summary>
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public ModelAccessToken()
        {

        }

        public ModelAccessToken(ModelUser user)
        {
            User = user;
        }
    }
}
