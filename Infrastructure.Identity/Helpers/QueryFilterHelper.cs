using System.Linq;
using Microsoft.EntityFrameworkCore;
using Application.Enums;
using Application.Interfaces;
using Application.Parameters;
using Infrastructure.Identity.Models;

namespace Infrastructure.Identity.Helpers
{
    /// <summary>
    /// Фильтры запросов
    /// </summary>
    public static class QueryFilterHelper
    {
        /// <summary>
        /// Фильтрация для пользователей
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<ModelUser> FilterForUsers(this IQueryable<ModelUser> source, UserFilter filter)
        {
            IQueryable<ModelUser> query = source;

            if (!string.IsNullOrEmpty(filter.FirstName))
                query = query.Where(x => x.FirstName == filter.FirstName);

            if (!string.IsNullOrEmpty(filter.LastName))
                query = query.Where(x => x.LastName == filter.LastName);

            if (!string.IsNullOrEmpty(filter.MiddleName))
                query = query.Where(x => x.MiddleName == filter.MiddleName);

            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(x => x.Email == filter.Email);

            if (!string.IsNullOrEmpty(filter.PhoneNumber))
                query = query.Where(x => x.PhoneNumber == filter.PhoneNumber);

            return query;
        }

        /// <summary>
        /// Фильтрация для токенов доступа
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<ModelAccessToken> FilterForTokens(this IQueryable<ModelAccessToken> source, TokenFilter filter)
        {
            IQueryable<ModelAccessToken> query = source;

            if (!string.IsNullOrEmpty(filter.Token))
                query = query.Where(x => x.Token == filter.Token);

            if (filter.Created is not null)
                query = query.Where(x => x.Created == filter.Created);

            if (!string.IsNullOrEmpty(filter.CreatedByBrowser))
                query = query.Where(x => x.CreatedByBrowser == filter.CreatedByBrowser);

            if (!string.IsNullOrEmpty(filter.CreatedByIp))
                query = query.Where(x => x.CreatedByIp == filter.CreatedByIp);

            if (filter.Expires is not null)
                query = query.Where(x => x.Expires == filter.Expires);

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(x => x.UserId == filter.UserId);

            return query;
        }

        /// <summary>
        /// Фильтрация для токенов доступа
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IQueryable<ModelRefreshToken> FilterForTokens(this IQueryable<ModelRefreshToken> source, TokenFilter filter)
        {
            IQueryable<ModelRefreshToken> query = source;

            if (!string.IsNullOrEmpty(filter.Token))
                query = query.Where(x => x.Token == filter.Token);

            if (filter.Created is not null)
                query = query.Where(x => x.Created == filter.Created);

            if (!string.IsNullOrEmpty(filter.CreatedByBrowser))
                query = query.Where(x => x.CreatedByBrowser == filter.CreatedByBrowser);

            if (!string.IsNullOrEmpty(filter.CreatedByIp))
                query = query.Where(x => x.CreatedByIp == filter.CreatedByIp);

            if (filter.Expires is not null)
                query = query.Where(x => x.Expires == filter.Expires);

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(x => x.UserId == filter.UserId);

            return query;
        }

        /// <summary>
        /// Фильтрация для отсеивания данных по организациям (метод отключения для супер админа)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static IQueryable<T> FilterBySuperAdmin<T>(this DbSet<T> source, ICurrentUser currentUser) where T : class
        {
            return CheckSuperAdmin(currentUser) ? source.IgnoreQueryFilters() : source;
        }

        /// <summary>
        /// Есть ли у пользователя роль супер админа
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        private static bool CheckSuperAdmin(ICurrentUser currentUser)
        {
            if (currentUser.Roles.Contains(Roles.SuperAdmin.ToString())) return true;
            return false;
        }
    }
}
