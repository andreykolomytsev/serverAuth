using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Enums;
using Application.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Helpers;
using Infrastructure.Identity.Contexts;

namespace Infrastructure.Identity.Seeds
{
    public static class DefaultSuperAdmin
    {
        public static async Task SeedAsync(ApplicationDbContext dbContext)
        {
            var allRoles = await dbContext.Roles.IgnoreQueryFilters().ToListAsync();
            var tenant = dbContext.Tenants.IgnoreQueryFilters().FirstOrDefault(x => x.Id == "780c0562-3e70-40ad-ae3b-3a1e444bc562");

            if (!allRoles.Any(x => x.Name == Roles.SuperAdmin.ToString()))
            {
                var defaultAdmin = new ModelRole(Roles.SuperAdmin.ToString(), tenant.Id, "Роль супер пользователя по умолчанию (Запрещены все настройки)");
                await dbContext.Roles.AddAsync(defaultAdmin);
                await dbContext.SaveChangesAsync();

                await SeedClaimsForSuperAdmin(dbContext, tenant);
            }

            //Создаем пользователя по умолчанию
            var defaultUser = new ModelUser
            {
                Email = "admin@gmail.com",
                FirstName = "Андрей",
                LastName = "Коломыцев",
                MiddleName = "",
                CreatedOn = DateTime.Now,
                IsActive = true,
                TenantId = tenant.Id,
                PasswordHash = PasswordHelper.CreatePasswordHash("Qwerty1234$!")
            };

            var user = await dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Email == defaultUser.Email);
            if (user is null)
            {
                await dbContext.Users.AddAsync(defaultUser);
                await dbContext.SaveChangesAsync();

                await dbContext.UserRoles.AddAsync(new UserRole() { UserId = defaultUser.Id, RoleId = dbContext.Roles.IgnoreQueryFilters().FirstOrDefault(x => x.Name == Roles.SuperAdmin.ToString())?.Id });
                await dbContext.SaveChangesAsync();
            }
        }

        public async static Task SeedClaimsForSuperAdmin(ApplicationDbContext dbContext, ModelTenant tenant)
        {
            var adminRole = await dbContext.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Name == Roles.SuperAdmin.ToString());

            if (adminRole is null)
                return;

            foreach (var permission in AllPermissions.GetRegisteredPermissions())
            {
                await dbContext.Permissions.AddAsync(new ModelPermission() { ClaimType = AllPermissionTypes.Permission, ClaimValue = permission });
            }

            await dbContext.SaveChangesAsync();


            var allPermissions = await dbContext.Permissions.IgnoreQueryFilters().ToListAsync();

            foreach (var permission in allPermissions)
            {
                await dbContext.PermissionRoles.AddAsync(new PermissionRole() { PermissionId = permission.Id, RoleId = adminRole.Id });
                await dbContext.PermissionTenants.AddAsync(new PermissionTenant() { PermissionId = permission.Id, TenantId = tenant.Id });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
