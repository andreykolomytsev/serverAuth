using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Application.Enums;
using Application.Wrappers;
using Application.Interfaces;
using Application.Parameters;
using Application.DTOs.Permissions;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Helpers;

namespace Infrastructure.Identity.Managers
{
    public class PermissionManager : IPermissionManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public PermissionManager(ApplicationDbContext dbContext, ICurrentUser currentUser, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        #region CRUD
        public async Task<PaginatedResult<ResponsePermission>> GetAllPermissionsAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var totalCount = await _dbContext.Permissions.CountAsync();

            var pagedData = await _dbContext.Permissions.OrderBy(x => x.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            return PaginatedResult<ResponsePermission>.Success(_mapper.Map<List<ResponsePermission>>(pagedData), totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IResult<ResponsePermission>> GetPermissionByIdAsync(string permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(x => x.Id == permissionId);

            if (permission is null)
                return await Result<ResponsePermission>.FailAsync(string.Format("Право доступа с ID [{0}] не существует", permissionId));

            return await Result<ResponsePermission>.SuccessAsync(_mapper.Map<ResponsePermission>(permission));
        }

        public async Task<IResult<ResponsePermission>> CreatePermissionAsync(RequestPermission request)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(x => x.ClaimType == request.Type && x.ClaimValue == request.Value);

            if (permission is not null)
                return await Result<ResponsePermission>.FailAsync(string.Format("Право доступа [{0}] уже существует", request.Value));

            await _dbContext.Permissions.AddAsync(_mapper.Map<ModelPermission>(request));
            await _dbContext.SaveChangesAsync();

            return await Result<ResponsePermission>.SuccessAsync(string.Format("Право доступа [{0}] добавлено", request.Value));
        }

        public async Task<IResult<ResponsePermission>> UpdatePermissionAsync(RequestPermission request, string permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(x => x.Id == permissionId);

            if (permission is null)
                return await Result<ResponsePermission>.FailAsync(string.Format("Право доступа с ID [{0}] не существует", permissionId));

            permission.ClaimType = request.Type;
            permission.ClaimValue = request.Value;

            _dbContext.Permissions.Update(permission);

            await _dbContext.SaveChangesAsync();

            return await Result<ResponsePermission>.SuccessAsync(string.Format("Право доступа [{0}] обновлено", request.Value));
        }

        public async Task<IResult<string>> DeletePermissionAsync(string permissionId)
        {
            var permission = await _dbContext.Permissions.FirstOrDefaultAsync(x => x.Id == permissionId);

            if (permission is null)
                return await Result<string>.FailAsync(string.Format("Право доступа с ID [{0}] не существует", permissionId));

            if (await _dbContext.PermissionRoles.AnyAsync(x => x.PermissionId == permissionId) || await _dbContext.PermissionTenants.AnyAsync(x => x.PermissionId == permissionId))
                return await Result<string>.FailAsync(string.Format("Право доступа с ID [{0}] используется. Удаление запрещено", permissionId));

            _dbContext.Permissions.Remove(permission);

            await _dbContext.SaveChangesAsync();

            return await Result<string>.SuccessAsync(string.Format("Право доступа [{0}] удалено", permission.ClaimValue));
        }
        #endregion

        #region ROLE-PERMISSIONS
        public async Task<IResult<ResponsePermissionRole>> GetAllPermissionsByRoleIdAsync(string roleId)
        {
            var role = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == roleId);

            if (role is null)
                return await Result<ResponsePermissionRole>.FailAsync(string.Format("Роль с ID [{0}] не существует", roleId));

            // получаем все права доступа для организации пользователя
            var allPermissions = await _dbContext.Permissions.Where(x => x.Tenants.Any(a => a.TenantId == role.TenantId)).ToListAsync();

            // получаем права доступа для роли
            var rolePermissions = await _dbContext.Permissions.Where(x => x.Roles.Any(a => a.RoleId == roleId)).ToListAsync();

            var responsePermissions = new List<ResponsePermission>();

            foreach (var permission in allPermissions)
            {
                responsePermissions.Add(new ResponsePermission()
                {
                    Id = permission.Id,
                    Type = permission.ClaimType,
                    Value = permission.ClaimValue,
                    Selected = rolePermissions.Any(a => a.Id == permission.Id)
                });
            }

            var responseModel = new ResponsePermissionRole()
            {
                RoleName = role.Name,
                Permissions = responsePermissions
            };

            return await Result<ResponsePermissionRole>.SuccessAsync(responseModel);
        }

        public async Task<IResult<ResponsePermissionRole>> UpdatePermissionsForRoleAsync(RequestPermissionRole request, string roleId)
        {
            var role = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == roleId);

            if (role is null)
                return await Result<ResponsePermissionRole>.FailAsync(string.Format("Роль с ID [{0}] не существует", roleId));

            if (role.Name == Roles.SuperAdmin.ToString())
                return await Result<ResponsePermissionRole>.FailAsync("Нельзя изменить права доступа для выбранной роли");

            // получаем все права доступа для организации пользователя
            var allPermissions = await _dbContext.Permissions.Where(x => x.Tenants.Any(a => a.TenantId == role.TenantId)).ToListAsync();

            // модель с новыми правами доступа
            var newRolePermission = new List<PermissionRole>();

            foreach (var permission in request.PermissionRoles)
            {
                if (!allPermissions.Any(x => x.Id == permission.PermissionId))
                    return await Result<ResponsePermissionRole>.FailAsync($"Права доступа с ID [{permission.PermissionId}] не существует");

                newRolePermission.Add(new PermissionRole() { PermissionId = permission.PermissionId, RoleId = roleId });
            }

            // получаем права доступа для роли
            var rolePermissionIds = await _dbContext.PermissionRoles.Where(x => x.RoleId == roleId).ToListAsync();

            if (!rolePermissionIds.Select(s => s.PermissionId).SequenceEqual(request.PermissionRoles.Select(s => s.PermissionId)))
            {
                _dbContext.PermissionRoles.RemoveRange(rolePermissionIds);

                await _dbContext.PermissionRoles.AddRangeAsync(newRolePermission);

                var users = await _dbContext.Users.FilterBySuperAdmin(_currentUser).Include(t => t.AccessTokens).Where(x => x.Roles.Any(x => x.RoleId == roleId)).AsSplitQuery().ToListAsync();

                // отмечаем токены что права поменялись
                foreach (var tokens in users.Select(s => s.AccessTokens))
                {
                    foreach (var token in tokens)
                    {
                        token.IsOutDated = true;

                        _dbContext.AccessTokens.Update(token);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            // модель для ответа (список прав доступа)
            var responsePermissions = new List<ResponsePermission>();

            foreach (var item in allPermissions)
            {
                responsePermissions.Add(new ResponsePermission()
                {
                    Id = item.Id,
                    Type = item.ClaimType,
                    Value = item.ClaimValue,
                    Selected = rolePermissionIds.Any(a => a.PermissionId == item.Id)
                });
            }

            // модель для ответа
            var response = new ResponsePermissionRole()
            {
                RoleName = role.Name,
                Permissions = responsePermissions
            };

            return await Result<ResponsePermissionRole>.SuccessAsync(response, string.Format("Права доступа для роли [{0}] обновлены", role.Name));
        }
        #endregion

        #region TENANT-PERMISSIONS
        public async Task<IResult<ResponsePermissionTenant>> GetAllPermissionsByTenantIdAsync(string tenantId)
        {
            var tenant = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == tenantId);

            if (tenant is null)
                return await Result<ResponsePermissionTenant>.FailAsync(string.Format("Организация с ID [{0}] не существует", tenantId));

            // получаем все права доступа
            var allPermissions = await _dbContext.Permissions.Where(x => x.Tenants.Any(a => a.TenantId == _currentUser.TenantId)).ToListAsync();

            // получаем права доступа для организации
            var tenantPermissions = await _dbContext.Permissions.Where(x => x.Tenants.Any(a => a.TenantId == tenantId)).ToListAsync();

            var responsePermissions = new List<ResponsePermission>();

            foreach (var permission in allPermissions)
            {
                responsePermissions.Add(new ResponsePermission()
                {
                    Id = permission.Id,
                    Type = permission.ClaimType,
                    Value = permission.ClaimValue,
                    Selected = tenantPermissions.Any(a => a.Id == permission.Id)
                });
            }

            var responseModel = new ResponsePermissionTenant()
            {
                TenantName = tenant.FullName,
                Permissions = responsePermissions
            };

            return await Result<ResponsePermissionTenant>.SuccessAsync(responseModel);
        }

        public async Task<IResult<ResponsePermissionTenant>> UpdatePermissionsForTenantAsync(RequestPermissionTenant request, string tenantId)
        {
            var tenant = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == tenantId);

            if (tenant is null)
                return await Result<ResponsePermissionTenant>.FailAsync(string.Format("Организация с ID [{0}] не существует", tenantId));

            if (tenant.Id == "780c0562-3e70-40ad-ae3b-3a1e444bc562")
                return await Result<ResponsePermissionTenant>.FailAsync("Нельзя изменить права доступа для выбранной организации");

            if (tenant.Id == _currentUser.TenantId)
                return await Result<ResponsePermissionTenant>.FailAsync("Нельзя изменить права доступа своей организации");

            // получаем все права доступа
            var allPermissions = await _dbContext.Permissions.ToListAsync();

            // модель с новыми правами доступа
            var newTenantPermission = new List<PermissionTenant>();

            foreach (var permission in request.PermissionTenants)
            {
                if (!allPermissions.Any(x => x.Id == permission.PermissionId))
                    return await Result<ResponsePermissionTenant>.FailAsync($"Права доступа с ID [{permission.PermissionId}] не существует");

                newTenantPermission.Add(new PermissionTenant() { PermissionId = permission.PermissionId, TenantId = tenantId });
            }

            // получаем права доступа для организации
            var tenantPermissionIds = await _dbContext.PermissionTenants.Where(x => x.TenantId == tenantId).ToListAsync();

            if (!tenantPermissionIds.Select(s => s.PermissionId).SequenceEqual(request.PermissionTenants.Select(s => s.PermissionId)))
            {
                _dbContext.PermissionTenants.RemoveRange(tenantPermissionIds);

                await _dbContext.PermissionTenants.AddRangeAsync(newTenantPermission);

                var users = await _dbContext.Users.FilterBySuperAdmin(_currentUser).Include(t => t.AccessTokens).Where(x => x.TenantId == tenantId).AsSplitQuery().ToListAsync();

                // отмечаем токены что права поменялись
                foreach (var tokens in users.Select(s => s.AccessTokens))
                {
                    foreach (var token in tokens)
                    {
                        token.IsOutDated = true;

                        _dbContext.AccessTokens.Update(token);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            // модель для ответа (список прав доступа)
            var responsePermissions = new List<ResponsePermission>();

            foreach (var item in allPermissions)
            {
                responsePermissions.Add(new ResponsePermission()
                {
                    Id = item.Id,
                    Type = item.ClaimType,
                    Value = item.ClaimValue,
                    Selected = tenantPermissionIds.Any(a => a.PermissionId == item.Id)
                });
            }

            // модель для ответа
            var response = new ResponsePermissionTenant()
            {
                TenantName = tenant.FullName,
                Permissions = responsePermissions
            };

            return await Result<ResponsePermissionTenant>.SuccessAsync(response, string.Format("Права доступа для организации [{0}] обновлены", tenant.FullName));
        }
        #endregion
    }
}
