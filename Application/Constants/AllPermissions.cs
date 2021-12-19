using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Application.Constants
{
    public static class AllPermissions
    {
        [DisplayName("Users")]
        [Description("Users Permissions")]
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }

        [DisplayName("Roles")]
        [Description("Roles Permissions")]
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
        }

        [DisplayName("Tenants")]
        [Description("Tenants Permissions")]
        public static class Tenants
        {
            public const string View = "Permissions.Tenants.View";
            public const string Create = "Permissions.Tenants.Create";
            public const string Edit = "Permissions.Tenants.Edit";
            public const string Delete = "Permissions.Tenants.Delete";
        }

        [DisplayName("Services")]
        [Description("Services Permissions")]
        public static class Services
        {
            public const string View = "Permissions.Services.View";
            public const string Create = "Permissions.Services.Create";
            public const string Edit = "Permissions.Services.Edit";
            public const string Delete = "Permissions.Services.Delete";
        }

        [DisplayName("Tokens")]
        [Description("Tokens Permissions")]
        public static class Tokens
        {
            public const string View = "Permissions.Tokens.View";
            public const string Create = "Permissions.Tokens.Create";
            public const string Edit = "Permissions.Tokens.Edit";
            public const string Delete = "Permissions.Tokens.Delete";
        }

        [DisplayName("Role Permissions")]
        [Description("Role Permissions")]
        public static class RolePermissions
        {
            public const string View = "Permissions.RolePermissions.View";
            public const string Edit = "Permissions.RolePermissions.Edit";
        }

        [DisplayName("Tenant Permissions")]
        [Description("Tenant Permissions")]
        public static class TenantPermissions
        {
            public const string View = "Permissions.TenantPermissions.View";
            public const string Edit = "Permissions.TenantPermissions.Edit";
        }

        /// <summary>
        /// Возвращает список Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRegisteredPermissions()
        {
            var permissions = new List<string>();

            foreach (var prop in typeof(AllPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);

                if (propertyValue != null)
                    permissions.Add(propertyValue.ToString());
            }

            return permissions;
        }
    }
}
