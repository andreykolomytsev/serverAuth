using Application.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace WebApi.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<Roles> _roles;
        private readonly IList<string> _permissions;

        /// <summary>
        /// Конструктор с базовой авторизацией
        /// </summary>
        public AuthorizeAttribute()
        {

        }

        /// <summary>
        /// Конструктор с авторизацией на основе прав доступа
        /// </summary>
        /// <param name="permissions">Список прав доступа для авторизации</param>
        public AuthorizeAttribute(params string[] permissions)
        {
            _permissions = permissions ?? Array.Empty<string>();
        }

        /// <summary>
        /// Конструктор с авторизацией на основе ролей
        /// </summary>
        /// <param name="roles">Список ролей для авторизации</param>
        public AuthorizeAttribute(params Roles[] roles)
        {
            _roles = roles ?? Array.Empty<Roles>();
        }

        /// <summary>
        /// Конструктор с авторизацией на основе прав доступа и ролей
        /// </summary>
        /// <param name="roles">Список ролей для авторизации</param>
        /// <param name="permissions">Список прав доступа для авторизации</param>
        public AuthorizeAttribute(Roles[] roles, string[] permissions)
        {
            _roles = roles;
            _permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!CheckAuthorization(context))
            {
                context.Result = new ChallengeResult();
                return;
            }

            if (_permissions is not null) PermissionAuthorization(context);
            if (_roles is not null) RoleAuthorization(context);
        }

        /// <summary>
        /// Проверка не был ли токен отменен вручную
        /// </summary>
        /// <param name="context"></param>
        private static bool CheckAuthorization(AuthorizationFilterContext context)
        {
            var tokenIsActive = (bool)context.HttpContext.Items["ActiveToken"];

            if (!tokenIsActive)
                return false;

            return true;
        }

        /// <summary>
        /// Проверка авторизации для прав доступа
        /// </summary>
        /// <param name="context"></param>
        private void PermissionAuthorization(AuthorizationFilterContext context)
        {
            var permissions = context.HttpContext?.User?.Claims.Where(x => x.Type == Application.Constants.AllPermissionTypes.Permission).Select(s => s.Value).ToList();

            if (_permissions.Any() && !_permissions.Intersect(permissions).Any())
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        /// <summary>
        /// Проверка авторизации для ролей
        /// </summary>
        /// <param name="context"></param>
        private void RoleAuthorization(AuthorizationFilterContext context)
        {
            var roles = context.HttpContext?.User?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(s => s.Value).ToList();

            if (_roles.Any() && !_roles.Select(s => s.ToString()).ToList().Intersect(roles).Any())
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
