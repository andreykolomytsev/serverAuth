using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Application.Wrappers;
using Application.Interfaces;
using Application.Parameters;
using Application.DTOs.MicroService;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Models;
using Infrastructure.Identity.Helpers;
using Application.DTOs.Tenants;

namespace Infrastructure.Identity.Managers
{
    public class ServiceManager : IServiceManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public ServiceManager(ApplicationDbContext dbContext, ICurrentUser currentUser, IMapper mapper)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        #region CRUD
        public async Task<PaginatedResult<ResponseMS>> GetAllMSAsync(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var totalCount = await _dbContext.MicroServices.FilterBySuperAdmin(_currentUser).CountAsync();

            var allServices = await _dbContext.MicroServices.FilterBySuperAdmin(_currentUser).Include(i => i.Users).OrderBy(x => x.CreatedOn).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).ToListAsync();

            var pagedData = new List<ResponseMS>();

            foreach (var item in allServices)
            {
                pagedData.Add(new ResponseMS()
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    Description = item.Description,
                    IP = item.IP,
                    Port = item.Port,
                    URL = item.URL,
                    Selected = allServices.Where(x => x.Id == item.Id).Any(a => a.Users.Select(s => s.UserId).Contains(_currentUser.UserId))
                });
            }

            return PaginatedResult<ResponseMS>.Success(_mapper.Map<List<ResponseMS>>(pagedData), totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<IResult<ResponseMS>> GetMSByIdAsync(string msId)
        {
            var microService = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == msId);

            if (microService is null)
                return await Result<ResponseMS>.FailAsync(string.Format("Сервис с ID [{0}] не существует", msId));

            var result = _mapper.Map<ResponseMS>(microService);
            return await Result<ResponseMS>.SuccessAsync(result);
        }

        public async Task<IResult<ResponseMS>> CreateMSAsync(RequestMS requestMS)
        {
            var checkMS = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.FullName == requestMS.FullName);

            if (checkMS is not null)
                return await Result<ResponseMS>.FailAsync(string.Format("Сервис [{0}] уже существует", requestMS.FullName));

            var newMicroService = _mapper.Map<ModelService>(requestMS);

            await _dbContext.MicroServices.AddAsync(newMicroService);
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseMS>.SuccessAsync(_mapper.Map<ResponseMS>(newMicroService), string.Format("Сервис [{0}] зарегистрирован", newMicroService.FullName));
        }

        public async Task<IResult<ResponseMS>> UpdateMSAsync(RequestMS requestMS, string msId)
        {
            var microService = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == msId);

            if (microService is null)
                return await Result<ResponseMS>.FailAsync(string.Format("Сервис с ID [{0}] не существует", msId));

            if (microService.FullName != requestMS.FullName && await _dbContext.MicroServices.AnyAsync(x => x.FullName == requestMS.FullName))
                return await Result<ResponseMS>.FailAsync(string.Format("Сервис [{0}] уже существует", requestMS.FullName));

            _mapper.Map(requestMS, microService);

            _dbContext.MicroServices.Update(microService);
            await _dbContext.SaveChangesAsync();

            return await Result<ResponseMS>.SuccessAsync(_mapper.Map<ResponseMS>(microService), string.Format("Сервис [{0}] обновлен", requestMS.FullName));
        }

        public async Task<IResult<string>> DeleteMSAsync(string msId)
        {
            var microService = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == msId);

            if (microService is null)
                return await Result<string>.FailAsync(string.Format("Сервис с ID [{0}] не существует", msId));

            _dbContext.MicroServices.Remove(microService);
            await _dbContext.SaveChangesAsync();

            return await Result<string>.SuccessAsync(string.Format("Сервис [{0}] удален", microService.FullName));
        }
        #endregion

        #region USER-SERVICE
        public async Task<IResult<ResponseServiceUser>> GetAllUsersByService(string msId)
        {
            var microService = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == msId);

            if (microService is null)
                return await Result<ResponseServiceUser>.FailAsync(string.Format("Сервис с ID [{0}] не существует", msId));

            // получить список всех пользователей
            var allUsers = await _dbContext.Users.FilterBySuperAdmin(_currentUser).ToListAsync();

            // получаем пользователей для сервиса
            var userServices = await _dbContext.Users.FilterBySuperAdmin(_currentUser).Where(x => x.UserServices.Any(a => a.MicroServiceId == msId)).ToListAsync();

            var responseUsers = new List<ResponseUserService>();

            foreach (var user in allUsers)
            {
                var tenant = await _dbContext.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == user.TenantId);

                responseUsers.Add(new ResponseUserService()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = user.MiddleName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    Tenant = _mapper.Map<ResponseTenant>(tenant),
                    Selected = userServices.Any(a => a.Id == user.Id)
                });
            }

            var responseModel = new ResponseServiceUser()
            {
                ServiceName = microService.FullName,
                Users = responseUsers
            };

            return await Result<ResponseServiceUser>.SuccessAsync(responseModel);
        }

        public async Task<IResult<ResponseServiceUser>> UpdateUsersByService(RequestServiceUser requestUsers, string msId)
        {
            var microService = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == msId);

            if (microService is null)
                return await Result<ResponseServiceUser>.FailAsync(string.Format("Сервис с ID [{0}] не существует", msId));

            // получить список всех пользователей
            var allUsers = await _dbContext.Users.FilterBySuperAdmin(_currentUser).ToListAsync();

            // модель с новыми пользователями
            var newUserService = new List<UserService>();

            foreach (var user in requestUsers.Users)
            {
                if (!allUsers.Any(x => x.Id == user.UserId))
                    return await Result<ResponseServiceUser>.FailAsync($"Пользователь с ID [{user.UserId}] не существует");

                newUserService.Add(new UserService() { MicroServiceId = msId, UserId = user.UserId });
            }

            // получаем пользователей для сервиса
            var userServicesIds = await _dbContext.UserMicroServices.FilterBySuperAdmin(_currentUser).Where(x => x.MicroServiceId == msId).ToListAsync();

            if (!userServicesIds.Select(s => s.UserId).SequenceEqual(requestUsers.Users.Select(s => s.UserId)))
            {
                _dbContext.UserMicroServices.RemoveRange(userServicesIds);

                await _dbContext.UserMicroServices.AddRangeAsync(newUserService);

                // отмечаем токены что права поменялись
                foreach (var currentUser in requestUsers.Users)
                {
                    var user = await _dbContext.Users.IgnoreQueryFilters().Include(x => x.AccessTokens).FirstOrDefaultAsync(x => x.Id == currentUser.UserId);

                    foreach (var token in user.AccessTokens.ToList())
                    {
                        token.IsOutDated = true;
                    }

                    _dbContext.Users.Update(user);
                }

                await _dbContext.SaveChangesAsync();
            }

            // модель для ответа (список пользователей)
            var responseUsers = new List<ResponseUserService>();

            foreach (var user in allUsers)
            {
                var tenant = await _dbContext.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == user.TenantId);

                responseUsers.Add(new ResponseUserService()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = user.MiddleName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    Tenant = _mapper.Map<ResponseTenant>(tenant),
                    Selected = userServicesIds.Any(a => a.UserId == user.Id)
                });
            }

            // модель для ответа
            var response = new ResponseServiceUser()
            {
                ServiceName = microService.FullName,
                Users = responseUsers
            };

            return await Result<ResponseServiceUser>.SuccessAsync(response, string.Format("Доступ пользователям к сервису [{0}] обновлен", microService.FullName));
        }
        #endregion

        #region TENANT-SERVICE
        public async Task<IResult<ResponseServiceTenant>> GetAllTenantsByService(string msId)
        {
            var microService = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == msId);

            if (microService is null)
                return await Result<ResponseServiceTenant>.FailAsync(string.Format("Сервис с ID [{0}] не существует", msId));

            // получить список всех организаций
            var allTenants = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).ToListAsync();

            // получаем организации для сервиса
            var tenantServices = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).Where(x => x.Services.Any(a => a.MicroServiceId == msId)).ToListAsync();

            var responseTenants = new List<ResponseTenantService>();

            foreach (var tenant in allTenants)
            {
                responseTenants.Add(new ResponseTenantService()
                {
                    Id = tenant.Id,
                    FullName = tenant.FullName,
                    INN = tenant.INN,
                    Email = tenant.Email,
                    Phone = tenant.Phone,
                    KPP = tenant.KPP,
                    OKPO = tenant.OKPO,
                    OGRN = tenant.OKPO,
                    Address = tenant.Address,
                    Selected = tenantServices.Any(a => a.Id == tenant.Id)
                });
            }

            var responseModel = new ResponseServiceTenant()
            {
                ServiceName = microService.FullName,
                Tenants = responseTenants
            };

            return await Result<ResponseServiceTenant>.SuccessAsync(responseModel);
        }

        public async Task<IResult<ResponseServiceTenant>> UpdateTenantsByService(RequestServiceTenant requestTenants, string msId)
        {
            var microService = await _dbContext.MicroServices.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == msId);

            if (microService is null)
                return await Result<ResponseServiceTenant>.FailAsync(string.Format("Сервис с ID [{0}] не существует", msId));

            // получить список всех организаций
            var allTenants = await _dbContext.Tenants.FilterBySuperAdmin(_currentUser).ToListAsync();

            // модель с новыми организациями
            var newTenantService = new List<TenantService>();

            foreach (var tenant in requestTenants.Tenants)
            {
                if (!allTenants.Any(x => x.Id == tenant.TenantId))
                    return await Result<ResponseServiceTenant>.FailAsync($"Организация с ID [{tenant.TenantId}] не существует");

                newTenantService.Add(new TenantService() { MicroServiceId = msId, TenantId = tenant.TenantId });
            }

            // получаем организации для сервиса
            var tenantServicesIds = await _dbContext.TenantMicroServices.FilterBySuperAdmin(_currentUser).Where(x => x.MicroServiceId == msId).ToListAsync();

            if (!tenantServicesIds.Select(s => s.TenantId).SequenceEqual(requestTenants.Tenants.Select(s => s.TenantId)))
            {
                _dbContext.TenantMicroServices.RemoveRange(tenantServicesIds);

                await _dbContext.TenantMicroServices.AddRangeAsync(newTenantService);

                await _dbContext.SaveChangesAsync();
            }

            // модель для ответа (список пользователей)
            var responseTenants = new List<ResponseTenantService>();

            foreach (var tenant in allTenants)
            {
                responseTenants.Add(new ResponseTenantService()
                {
                    Id = tenant.Id,
                    FullName = tenant.FullName,
                    INN = tenant.INN,
                    Email = tenant.Email,
                    Phone = tenant.Phone,
                    KPP = tenant.KPP,
                    OKPO = tenant.OKPO,
                    OGRN = tenant.OKPO,
                    Address = tenant.Address,
                    Selected = tenantServicesIds.Any(a => a.TenantId == tenant.Id)
                });
            }

            var responseModel = new ResponseServiceTenant()
            {
                ServiceName = microService.FullName,
                Tenants = responseTenants
            };

            return await Result<ResponseServiceTenant>.SuccessAsync(responseModel, string.Format("Доступ организациям к сервису [{0}] обновлен", microService.FullName));
        }
        #endregion
    }
}
