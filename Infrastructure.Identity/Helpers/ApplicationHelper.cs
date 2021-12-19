using System;
using System.Security.Cryptography;
using Infrastructure.Identity.Models;

namespace Infrastructure.Identity.Helpers
{
    public static class ApplicationHelper
    {
        /// <summary>
        /// Генерация рандомной строки
        /// </summary>
        /// <returns>hex string</returns>
        public static string RandomTokenString()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[40];
                rngCryptoServiceProvider.GetBytes(randomBytes);

                return BitConverter.ToString(randomBytes).Replace("-", "");
            };
        }

        /// <summary>
        /// Генерация токена обновления
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ipAddress"></param>
        /// <param name="browser"></param>
        /// <returns></returns>
        public static ModelRefreshToken GenerateRefreshToken(ModelUser user, string ipAddress, string browser)
        {
            return new ModelRefreshToken(user)
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7), // Продолжительность жизни - 7 дней
                Created = DateTime.Now,
                CreatedByIp = ipAddress,
                CreatedByBrowser = browser
            };
        }
    }
}
