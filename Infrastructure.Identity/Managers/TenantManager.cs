using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Tenants;
using Application.Interfaces;
using Application.Parameters;
using Application.Wrappers;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Helpers;
using Application.Enums;

namespace Infrastructure.Identity.Managers
{
    public class TenantManager : ITenantManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public TenantManager(ApplicationDbContext dbContext, ICurrentUser currentUser, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        #region CRUD
        public async Task<PaginatedResult<ResponseTenant>> GetAllTenantsAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            //var totalCount = await _dbContext.Tenants.IgnoreQueryFilters().Where(x => tenantList.Select(s => s.Id).Contains(x.TenantId)).CountAsync();

            //var pagedData = await _dbContext.Tenants.IgnoreQueryFilters().Where(x => tenantList.Select(s => s.Id).Contains(x.TenantId)).OrderBy(x => x.CreatedOn).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            //var parentTenant = await _dbContext.Tenants.Where(x => x.Id == _currentUser.TenantId).ToListAsync();
            var t2 = await _dbContext.Tenants.IgnoreQueryFilters().Where(x => x.TenantId == _currentUser.TenantId).ToListAsync();

            List<ModelTenant> t3 = new();

            foreach(var item in t2)
            {
                t3.AddRange(await _dbContext.Tenants.IgnoreQueryFilters().Where(x => x.TenantId == item.Id).ToListAsync());
            }


            var t4 = t2.Union(t3).ToList();

            //var parentTenants = await _dbContext.Tenants.IgnoreQueryFilters().Where(x => x.Id == x.TenantId).ToListAsync();

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

            //var totalCount = await _dbContext.Tenants.IgnoreQueryFilters().Where(x => tenantList.Select(s => s.Id).Contains(x.TenantId)).CountAsync();

            //var pagedData = await _dbContext.Tenants.IgnoreQueryFilters().Where(x => tenantList.Select(s => s.Id).Contains(x.TenantId)).OrderBy(x => x.CreatedOn).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            var totalCount = t4.Count();

            var pagedData = t4.OrderBy(x => x.CreatedOn).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToList();


            //foreach (var data in pagedData)
            //{
            //    data.Tenant = allTenants.FirstOrDefault(x => x.Id == data.TenantId);
            //}

            return PaginatedResult<ResponseTenant>.Success(_mapper.Map<List<ResponseTenant>>(pagedData), totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IResult<ResponseTenant>> GetTenantByIdAsync(string tenantId)
        {
            var tenant = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == tenantId);

            if (tenant is null)
                return await Result<ResponseTenant>.FailAsync(string.Format("Организация с ID [{0}] не существует", tenantId));

            var result = _mapper.Map<ResponseTenant>(tenant);
            return await Result<ResponseTenant>.SuccessAsync(result);
        }

        public async Task<IResult<ResponseTenant>> CreateTenantAsync(RequestTenant requestTenant)
        {
            var checkTenant = await _dbContext.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.INN == requestTenant.INN);

            if (checkTenant is not null)
                return await Result<ResponseTenant>.FailAsync(string.Format("Организация с ИНН [{0}] уже существует", requestTenant.INN));

            var newTenant = _mapper.Map<ModelTenant>(requestTenant);
            newTenant.TenantId = _currentUser.TenantId;

            var list = new List<TenantUser>() { new TenantUser() { TenantId = _currentUser.TenantId, UserId = _currentUser.UserId } };
            newTenant.Users = list;

            await _dbContext.Tenants.AddAsync(newTenant);
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseTenant>.SuccessAsync(_mapper.Map<ResponseTenant>(newTenant), string.Format("Организация [{0}] зарегистрирована", newTenant.FullName));
        }

        public async Task<IResult<ResponseTenant>> UpdateTenantAsync(string tenantId, RequestTenant requestTenant)
        {
            var tenant = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == tenantId);

            if (tenant is null)
                return await Result<ResponseTenant>.FailAsync(string.Format("Организация с ID [{0}] не существует", tenantId));

            if (tenant.INN != requestTenant.INN && await _dbContext.Tenants.AnyAsync(x => x.INN == requestTenant.INN))
                return await Result<ResponseTenant>.FailAsync(string.Format("Организация с ИНН [{0}] уже существует", requestTenant.INN));

            _mapper.Map(requestTenant, tenant);

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseTenant>.SuccessAsync(_mapper.Map<ResponseTenant>(tenant), string.Format("Организация [{0}] обновлена", tenant.FullName));
        }

        public async Task<IResult<string>> DeleteTenantAsync(string tenantId)
        {
            var tenant = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).FirstOrDefaultAsync(x => x.Id == tenantId);

            if (tenant is null)
                return await Result<string>.FailAsync(string.Format("Организация с ID [{0}] не существует", tenantId));

            if (await _dbContext.Users.IgnoreQueryFilters().AnyAsync(x => x.TenantId == tenantId))
                return await Result<string>.FailAsync(string.Format("Организация с ID [{0}] используется. Удаление запрещено", tenantId));

            if (await _dbContext.Roles.IgnoreQueryFilters().AnyAsync(x => x.TenantId == tenantId))
                return await Result<string>.FailAsync(string.Format("Организация с ID [{0}] используется. Удаление запрещено", tenantId));

            _dbContext.Tenants.Remove(tenant);

            await _dbContext.SaveChangesAsync();

            return await Result<string>.SuccessAsync(string.Format("Организация [{0}] удалена", tenant.FullName));
        }
        #endregion
    }
}
