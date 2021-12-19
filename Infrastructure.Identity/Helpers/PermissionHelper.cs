using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Application.Constants;
using Application.DTOs.Permissions;
using Infrastructure.Identity.Models;

namespace Infrastructure.Identity.Helpers
{
    public static class PermissionHelper
    {
        /// <summary>
        /// Список всех прав доступа
        /// </summary>
        /// <param name="allPermissions"></param>
        public static void GetAllPermissions(this List<ResponsePermission> allPermissions)
        {
            var modules = typeof(AllPermissions).GetNestedTypes();

            foreach (var module in modules)
            {
                var moduleName = string.Empty;

                if (module.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                    .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
                    moduleName = displayNameAttribute.DisplayName;

                var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                foreach (var fi in fields)
                {
                    var propertyValue = fi.GetValue(null);

                    if (propertyValue is not null)
                        allPermissions.Add(new ResponsePermission { Value = propertyValue.ToString(), Type = AllPermissionTypes.Permission });
                }
            }
        }

        /// <summary>
        /// Получить права доступа для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Права доступа</returns>
        public static IEnumerable<Claim> GetPermissionsByUser(ModelUser user, List<ModelRole> roles, List<ModelPermission> permissions, List<ModelService> services)
        {
            var roleClaims = new List<Claim>(); // роли пользователя
            var permissionClaims = new List<Claim>(); // права доступа для пользователя
            var serviceClaims = new List<Claim>(); // права доступа к микросервисам

            int countPermissionForPrevRole = 0;

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role.Name));

                var allPermissionsForThisRoles = permissions.Where(x => x.Roles.Any(a => a.RoleId == role.Id)).ToList();

                // Права доступа для роли берутся по наименьшему количеству доступных прав
                if (countPermissionForPrevRole <= allPermissionsForThisRoles.Count())
                {
                    foreach (var permisstion in allPermissionsForThisRoles)
                    {
                        if (!permissionClaims.Any(a => a.Value == permisstion.ClaimValue))
                        {
                            permissionClaims.Add(new Claim(permisstion.ClaimType, permisstion.ClaimValue));
                        }
                    }
                }
                else
                {
                    permissionClaims.Clear();

                    foreach (var permisstion in allPermissionsForThisRoles)
                    {
                        if (!permissionClaims.Any(a => a.Value == permisstion.ClaimValue))
                        {
                            permissionClaims.Add(new Claim(permisstion.ClaimType, permisstion.ClaimValue));
                        }
                    }
                }

                countPermissionForPrevRole = allPermissionsForThisRoles.Count();
            }

            foreach (var service in services)
            {
                serviceClaims.Add(new Claim(AllPermissionTypes.Service, service.Id));
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(AllPermissionTypes.Tenant, user.TenantId)
            }
            .Union(roleClaims) // роли
            .Union(permissionClaims) // права доступа
            .Union(serviceClaims); // Id доступных сервисов

            return claims;
        }
    }
}
