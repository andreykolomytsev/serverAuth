using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Users;
using Application.Enums;
using Application.Interfaces;
using Application.Parameters;
using Application.Wrappers;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Helpers;
using Infrastructure.Identity.Models;
using Application.Constants;

namespace Infrastructure.Identity.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public UserManager(ApplicationDbContext dbContext, ICurrentUser currentUser, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        #region CRUD
        public async Task<PaginatedResult<ResponseUser>> GetAllUsersAsync(PaginationFilter pagination, UserFilter filter)
        {
            var validFilter = new PaginationFilter(pagination.PageNumber, pagination.PageSize);

            //// получаем все организации
            //var allTenants = await _dbContext.Tenants.IgnoreQueryFilters().ToListAsync();

            //List<ModelTenant> tenantList = new();

            //if (_currentUser.Roles.Contains(Roles.SuperAdmin.ToString()))
            //{
            //    tenantList.AddRange(allTenants);
            //}
            //else
            //{
            //    tenantList.Add(allTenants.FirstOrDefault(x => x.Id == _currentUser.TenantId));
            //    tenantList.AddRange(allTenants.Where(x => x.TenantId == _currentUser.TenantId));
            //}

            //var availableTenants = await _dbContext.Tenants.Include(i => i.Users).Where(x => x.Users.Any(u => tenantList.Select(s => s.Id).Contains(u.TenantId))).ToListAsync();


            //int totalCount = await _dbContext.Users.Include(i => i.Tenants).IgnoreQueryFilters().Where(x => x.Tenants.Any(t => availableTenants.Select(s => s.Id).Contains(t.TenantId))).FilterForUsers(filter).CountAsync();

            //var pagedData = await _dbContext.Users.Include(i => i.Tenants).IgnoreQueryFilters().Where(x => x.Tenants.Any(t => availableTenants.Select(s => s.Id).Contains(t.TenantId))).FilterForUsers(filter).AsSplitQuery().OrderBy(x => x.CreatedOn).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();


            // получаем все организации
            var allTenants = await _dbContext.Tenants.IgnoreQueryFilters().ToListAsync();

            List<ModelTenant> tenantList = new();

            if (_currentUser.Roles.Contains(Roles.SuperAdmin.ToString()))
            {
                tenantList.AddRange(allTenants);
            }
            else
            {
                tenantList.Add(allTenants.FirstOrDefault(x => x.Id == _currentUser.TenantId));
                tenantList.AddRange(allTenants.Where(x => x.TenantId == _currentUser.TenantId));
            }

            var tenantsForQuery = await _dbContext.Tenants.IgnoreQueryFilters().Where(x => tenantList.Select(s => s.Id).Contains(x.TenantId)).ToListAsync();

            int totalCount = await _dbContext.Users.IgnoreQueryFilters().Where(x => tenantsForQuery.Select(s => s.Id).Contains(x.TenantId)).FilterForUsers(filter).CountAsync();

            var pagedData = await _dbContext.Users.IgnoreQueryFilters().Where(x => tenantsForQuery.Select(s => s.Id).Contains(x.TenantId)).FilterForUsers(filter).AsSplitQuery().OrderBy(x => x.CreatedOn).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            foreach (var data in pagedData)
            {
                data.Tenant = allTenants.FirstOrDefault(x => x.Id == data.TenantId);
            }

            return PaginatedResult<ResponseUser>.Success(_mapper.Map<List<ResponseUser>>(pagedData), totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<IResult<ResponseUser>> GetUserByIdAsync(string userId)
        {
            var user = await _dbContext.Users.FilterBySuperAdmin(_currentUser).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return await Result<ResponseUser>.FailAsync(string.Format("Пользователь с ID [{0}] не существует", userId));

            var allTenants = await _dbContext.Tenants.IgnoreQueryFilters().ToListAsync();

            user.Tenant = allTenants.FirstOrDefault(x => x.Id == user.TenantId);

            return await Result<ResponseUser>.SuccessAsync(_mapper.Map<ResponseUser>(user));
        }

        public async Task<IResult<ResponseUser>> CreateUserAsync(RequestCreateUser request)
        {
            var checkEmail = await _dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Email == request.Email);

            if (checkEmail is not null)
                return await Result<ResponseUser>.FailAsync(string.Format("Электронный адрес [{0}] уже существует", request.Email));

            var newUser = new ModelUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MiddleName = request.MiddleName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.IsActive,
                PasswordHash = PasswordHelper.CreatePasswordHash(request.Password),
                TenantId = (_currentUser.Permissions.Contains(AllPermissions.Tenants.View) && !string.IsNullOrEmpty(request.TenantId)) ? request.TenantId : _currentUser.TenantId
            };

            await _dbContext.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseUser>.SuccessAsync(_mapper.Map<ResponseUser>(newUser), string.Format("Пользователь [{0}] зарегистрирован", newUser.FirstName));
        }

        public async Task<IResult<ResponseUser>> UpdateUserAsync(RequestUpdateUser request, string userId)
        {
            var user = await _dbContext.Users.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return await Result<ResponseUser>.FailAsync(string.Format("Пользователь с ID [{0}] не существует", userId));

            if (user.Email != request.Email && await _dbContext.Users.IgnoreQueryFilters().AnyAsync(x => x.Email == request.Email))
                return await Result<ResponseUser>.FailAsync(string.Format("Электронный адрес [{0}] уже существует", request.Email));

            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.MiddleName = request.MiddleName;
            user.PhoneNumber = request.PhoneNumber;
            user.IsActive = request.IsActive;

            if (_currentUser.Permissions.Contains(AllPermissions.Tenants.View) && !string.IsNullOrEmpty(request.TenantId))
            {
                if (userId == _currentUser.UserId && request.TenantId != user.TenantId)
                    return await Result<ResponseUser>.FailAsync("Нельзя изменить организацию себе");

                user.TenantId = request.TenantId;
            }
            

            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseUser>.SuccessAsync(_mapper.Map<ResponseUser>(user), string.Format("Пользователь [{0}] обновлен", request.FirstName));
        }

        public async Task<IResult<ResponseUser>> DeleteUserAsync(string userId)
        {
            var user = await _dbContext.Users.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return await Result<ResponseUser>.FailAsync(string.Format("Пользователь с ID [{0}] не существует", userId));

            _dbContext.Remove(user);

            await _dbContext.SaveChangesAsync();

            return await Result<ResponseUser>.SuccessAsync(string.Format("Пользователь [{0}] удален", user.FirstName));
        }
        #endregion

        #region USER-ROLES
        public async Task<IResult<ResponseUserRoles>> GetUserRolesAsync(string userId)
        {
            var user = await _dbContext.Users.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return await Result<ResponseUserRoles>.FailAsync(string.Format("Пользователь с ID [{0}] не существует", userId));

            var viewModel = new List<UserRoleModel>();

            var roles = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).Include(x => x.Users).AsSplitQuery().ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };

                if (roles.Any(x => x.Users.Select(s => s.UserId).Contains(userId) && x.Id == role.Id)) userRolesViewModel.Selected = true;

                viewModel.Add(userRolesViewModel);
            }

            var result = new ResponseUserRoles { UserRoles = viewModel };

            return await Result<ResponseUserRoles>.SuccessAsync(result);
        }

        public async Task<IResult<ResponseUserRoles>> UpdateUserRolesAsync(RequestUserRoles request, string userId)
        {
            var user = await _dbContext.Users.FilterBySuperAdmin(_currentUser).Include(i => i.AccessTokens).FirstOrDefaultAsync(x => x.Id == userId);

            if (user is null)
                return await Result<ResponseUserRoles>.FailAsync(string.Format("Пользователь с ID [{0}] не существует", userId));

            if (user.Email == "admin@gmail.com")
                return await Result<ResponseUserRoles>.FailAsync("Нельзя изменить роли выбранного пользователя");

            var userRolesIds = await _dbContext.UserRoles.FilterBySuperAdmin(_currentUser).Where(x => x.UserId == userId).ToListAsync(); // получаем список ID ролей пользователя

            var roles = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).ToListAsync(); // получаем список всех ролей

            var newRoles = new List<UserRole>();

            // Id роли супер пользователя
            var superAdminRoleId = await _dbContext.Roles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Name == Roles.SuperAdmin.ToString());

            var tenant = await _dbContext.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.INN == "123455678");

            // получаем список заданных ролей с запроса
            foreach (var checkRole in request.UserRoles.Where(x => x.RoleId != null))
            {
                if (!roles.Any(x => x.Id == checkRole.RoleId))
                    return await Result<ResponseUserRoles>.FailAsync($"Роль с ID [{checkRole.RoleId}] не существует");

                if (checkRole.RoleId == superAdminRoleId.Id && user.TenantId != tenant.Id)
                    return await Result<ResponseUserRoles>.FailAsync($"Делегировать роль суперадминистратора другой организации запрещено");

                newRoles.Add(new UserRole() { UserId = userId, RoleId = checkRole.RoleId });
            }

            // получаем список ролей пользователя
            var userRoles = roles.Where(x => userRolesIds.Select(s => s.RoleId).Contains(x.Id)).ToList();

            // проверям есть ли у пользователя права на выставление роли суперпользователю
            var userHasSuperAdminRole = _currentUser.Roles.Any(x => x == Roles.SuperAdmin.ToString());

            if (!userHasSuperAdminRole && request.UserRoles.Any(s => s.RoleId == superAdminRoleId.Id))
                return await Result<ResponseUserRoles>.FailAsync("Недостаточно прав");

            if (!userRoles.Select(s => s.Id).SequenceEqual(request.UserRoles.Select(s => s.RoleId).ToList()))
            {
                // удаляем все роли у пользователя
                _dbContext.UserRoles.RemoveRange(userRolesIds);

                // добавляем заданные роли пользователю
                await _dbContext.UserRoles.AddRangeAsync(newRoles);

                foreach (var token in user.AccessTokens.ToList())
                {
                    token.IsOutDated = true;
                }

                _dbContext.Users.Update(user);

                await _dbContext.SaveChangesAsync();
            }

            return await Result<ResponseUserRoles>.SuccessAsync("Роли обновлены");
        }
        #endregion
    }
}
