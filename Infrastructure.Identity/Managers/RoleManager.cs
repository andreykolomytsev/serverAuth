using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Enums;
using Application.DTOs.Roles;
using Application.Interfaces;
using Application.Parameters;
using Application.Wrappers;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Helpers;
using Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Managers
{
    public class RoleManager : IRoleManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public RoleManager(ApplicationDbContext dbContext, ICurrentUser currentUser, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        #region CRUD ROLES
        public async Task<PaginatedResult<ResponseRole>> GetAllRolesAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var totalCount = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).CountAsync();

            var pagedRoles = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).Include(i => i.Tenant).AsSplitQuery().OrderBy(x => x.CreatedOn).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            return PaginatedResult<ResponseRole>.Success(_mapper.Map<List<ResponseRole>>(pagedRoles), totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IResult<ResponseRole>> GetRoleByIdAsync(string roleId)
        {
            var role = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).Include(i => i.Tenant).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == roleId);

            if (role is null)
                return await Result<ResponseRole>.FailAsync(string.Format("Роль с ID [{0}] не существует", roleId));

            if (role.Name == Roles.SuperAdmin.ToString())
                return await Result<ResponseRole>.FailAsync("Запрещено");

            return await Result<ResponseRole>.SuccessAsync(_mapper.Map<ResponseRole>(role));
        }

        public async Task<IResult<ResponseRole>> CreateRoleAsync(RequestRole request)
        {
            var role = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Name == request.Name);

            if (role is not null)
                return await Result<ResponseRole>.FailAsync(string.Format("Роль [{0}] уже существует", role.Name));

            if (request.Name == Roles.SuperAdmin.ToString())
                return await Result<ResponseRole>.FailAsync("Запрещено");

            await _dbContext.Roles.AddAsync(new ModelRole(request.Name, _currentUser.TenantId, request.Description));
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseRole>.SuccessAsync(_mapper.Map<ResponseRole>(role), string.Format("Роль [{0}] добавлена", request.Name));
        }

        public async Task<IResult<ResponseRole>> UpdateRoleAsync(RequestRole request, string roleId)
        {
            var role = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == roleId);

            if (role is null)
                return await Result<ResponseRole>.FailAsync(string.Format("Роль с ID [{0}] не существует", roleId));

            if (role.Name == Roles.SuperAdmin.ToString())
                return await Result<ResponseRole>.FailAsync("Запрещено");

            role.Name = request.Name;
            role.Description = request.Description;

            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseRole>.SuccessAsync(string.Format("Роль [{0}] обновлена", request.Name));
        }

        public async Task<IResult<string>> DeleteRoleAsync(string roleId)
        {
            var role = await _dbContext.Roles.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == roleId);

            if (role is null)
                return await Result<string>.FailAsync(string.Format("Роль с ID [{0}] не существует", roleId));

            if (role.Name == Roles.SuperAdmin.ToString())
                return await Result<string>.FailAsync("Запрещено");

            bool roleIsNotUsed = true;

            var allUsers = await _dbContext.Users.FilterBySuperAdmin(_currentUser).ToListAsync();

            foreach (var user in allUsers)
            {
                if (await _dbContext.UserRoles.AnyAsync(x => x.UserId == user.Id && x.RoleId == roleId)) roleIsNotUsed = false;
            }

            if (!roleIsNotUsed)
                return await Result<string>.SuccessAsync(string.Format("Роль [{0}] сейчас используется. Удаление запрещено", role.Name));

            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();

            return await Result<string>.SuccessAsync(string.Format("Роль [{0}] удалена", role.Name));
        }
        #endregion
    }
}
